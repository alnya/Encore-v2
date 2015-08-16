namespace Encore.PoolParty
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using Encore.Domain.Interfaces.Services;
    using System.Threading.Tasks;
    using System.Threading;
    using Encore.Domain.Entities;
    using log4net;

    public class PoolPartyClient : IProvideFieldData
    {
        private readonly ILog log = LogManager.GetLogger(typeof(PoolPartyClient));

        private readonly string projectUrl;
        private readonly string userName;
        private readonly string password;

        private const string ConceptQuery =
@"PREFIX skos:<http://www.w3.org/2004/02/skos/core#>
SELECT ?id ?name ?broader
WHERE {
    ?id skos:prefLabel ?name.
    OPTIONAL 
    { 
        ?id skos:broader ?broader 
    }
 }";

        private const string ConceptRelationsQuery =
@"PREFIX skos:<http://www.w3.org/2004/02/skos/core#>
SELECT ?id  ?relatedId
WHERE {
    ?id skos:related ?relatedId;
}";

        private const string ProjectIdQuery =
@"PREFIX skos:<http://www.w3.org/2004/02/skos/core#>
SELECT ?id  ?projectId
WHERE {
    ?id skos:hiddenLabel ?projectId;
}";

        private const string AlternativeLabelQuery =
@"PREFIX skos:<http://www.w3.org/2004/02/skos/core#>
SELECT ?id ?name
WHERE {
    ?id skos:altLabel ?name;
}";

        private class ConceptResult
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string BroaderId { get; set; }
        }

        private class ConceptRelation
        {
            public string Id { get; set; }
            public string RelatedId { get; set; }
        }

        private class ProjectIdResult
        {
            public string FieldId { get; set; }
            public string ProjectId { get; set; }
        }

        private class AlternativeLabelResult
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        public PoolPartyClient(string projectUrl, string userName, string password)
        {
            this.projectUrl = projectUrl;
            this.userName = userName;
            this.password = password;
        }

        public async Task<IEnumerable<Field>> GetFieldsAsync(CancellationToken token)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(projectUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var byteArray = Encoding.ASCII.GetBytes(String.Format("{0}:{1}", userName, password));
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                List<ConceptResult> requestConcepts = await RequestConceptsAsync(client, token);
                List<ConceptRelation> requestRelations = await RequestConceptRelationsAsync(client, token);
                List<ProjectIdResult> requestProjectIds = await RequestProjectIdsAsync(client, token);
                List<AlternativeLabelResult> requestAlternativeLabels = await RequestAlternativeLabelsAsync(client, token);

                if (requestConcepts != null && requestRelations != null && requestProjectIds != null && requestAlternativeLabels != null)
                {
                    return CreateFields(
                        requestConcepts,
                        requestRelations,
                        requestProjectIds,
                        requestAlternativeLabels);
                }
            }

            return null;
        }

        private IEnumerable<Field> CreateFields(
            List<ConceptResult> requestConcepts, 
            List<ConceptRelation> conceptRelations, 
            List<ProjectIdResult> requestProjectIds,
            List<AlternativeLabelResult> requestAlternativeLabels)
        {
            ConceptResult allMeasurements = requestConcepts.FirstOrDefault(x => String.Equals(x.Name, "measurements", StringComparison.OrdinalIgnoreCase));
            ConceptResult allUnits = requestConcepts.FirstOrDefault(x => String.Equals(x.Name, "units", StringComparison.OrdinalIgnoreCase));

            if (allMeasurements == null || allUnits == null)
            {
                log.Error("Measurements or Units concept not found");
                return null;
            }

            var fields = new List<Field>();

            var fieldCategories = requestConcepts.Where(x => x.BroaderId == allMeasurements.Id).ToList();
            var units = requestConcepts.Where(x => x.BroaderId == allUnits.Id).ToList();

            foreach (var conecpt in requestConcepts)
            {
                var parentCategory = fieldCategories.FirstOrDefault(x => x.Id == conecpt.BroaderId);
                var relations = conceptRelations.Where(x => x.Id == conecpt.Id);
                var unit = units.FirstOrDefault(u => relations.Any(r => r.RelatedId == u.Id));

                if(parentCategory != null && unit != null)
                {
                    var unitAltName = requestAlternativeLabels.FirstOrDefault(x => x.Id == unit.Id);
                    var fieldAltNames = requestAlternativeLabels.Where(x => x.Id == conecpt.Id);

                    var field = new Field
                    {
                        SourceId = conecpt.Id,
                        Name = conecpt.Name,
                        Type = parentCategory.Name,
                        Unit = unitAltName != null ? unitAltName.Name : unit.Name,
                        AlternativeNames = fieldAltNames.Select(x => x.Name).ToList(),
                        ProjectIds = requestProjectIds.Where(x => x.FieldId == conecpt.Id).Select(x => x.ProjectId).ToList()
                    };

                    fields.Add(field);
                }
            }

            log.InfoFormat("Found and returning {0} Fields", fields.Count);

            return fields;
        }

        private async Task<List<ConceptResult>> RequestConceptsAsync(HttpClient client, CancellationToken token)
        {
            log.Info("Getting Concepts from Pool Party");

            var queryResult = await RequestDynamicResultAsync(client, ConceptQuery, token);

            if(queryResult != null)
            {
                var conceptResults = new List<ConceptResult>();

                foreach (var result in queryResult.results.bindings)
                {
                    conceptResults.Add(new ConceptResult
                    {
                        Id = result.id.value,
                        Name = result.name.value,
                        BroaderId = result.broader == null ? "" : result.broader.value
                    });
                }

                return conceptResults;
            }            
            
            return null;
        }

        private async Task<List<ConceptRelation>> RequestConceptRelationsAsync(HttpClient client, CancellationToken token)
        {
            log.Info("Getting ConceptRelations from Pool Party");

            var queryResult = await RequestDynamicResultAsync(client, ConceptRelationsQuery, token);

            if (queryResult != null)
            {
                var relationResults = new List<ConceptRelation>();

                foreach (var result in queryResult.results.bindings)
                {
                    relationResults.Add(new ConceptRelation
                    {                       
                        Id = result.id.value,
                        RelatedId = result.relatedId.value
                    });
                }

                return relationResults;
            }

            return null;
        }

        private async Task<List<ProjectIdResult>> RequestProjectIdsAsync(HttpClient client, CancellationToken token)
        {
            log.Info("Getting Field project Ids from Pool Party");

            var queryResult = await RequestDynamicResultAsync(client, ProjectIdQuery, token);

            if (queryResult != null)
            {
                var idResults = new List<ProjectIdResult>();

                foreach (var result in queryResult.results.bindings)
                {
                    idResults.Add(new ProjectIdResult
                    {
                        FieldId = result.id.value,
                        ProjectId = result.projectId.value
                    });
                }

                return idResults;
            }

            return null;
        }

        private async Task<List<AlternativeLabelResult>> RequestAlternativeLabelsAsync(HttpClient client, CancellationToken token)
        {
            log.Info("Getting Concept alternative labels from Pool Party");

            var queryResult = await RequestDynamicResultAsync(client, AlternativeLabelQuery, token);

            if (queryResult != null)
            {
                var labelResults = new List<AlternativeLabelResult>();

                foreach (var result in queryResult.results.bindings)
                {
                    labelResults.Add(new AlternativeLabelResult
                    {
                        Id = result.id.value,
                        Name = result.name.value
                    });
                }

                return labelResults;
            }

            return null;
        }
        
        private async Task<dynamic> RequestDynamicResultAsync(HttpClient client, string query, CancellationToken token)
        {
            var requestContent = new FormUrlEncodedContent(new[] 
                {
                    new KeyValuePair<string, string>("query", query)
                });

            HttpResponseMessage response = await client.PostAsync("", requestContent, token);

            if (response.IsSuccessStatusCode)
            {
                HttpContent responseContent = response.Content;
                string resultString = await responseContent.ReadAsStringAsync();
                return Newtonsoft.Json.JsonConvert.DeserializeObject(resultString);
            }

            log.ErrorFormat("Error: Pool Party responded with status {0}, messge {1}", response.StatusCode, response.ReasonPhrase);
            return null;
        }
    }
}
