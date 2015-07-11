namespace Encore.Web.Modules
{
    using Nancy;
    using Nancy.Responses.Negotiation;
    using Nancy.Validation;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using AutoMapper;
    using Encore.Domain.Interfaces.Services;
    using Encore.Domain.Services.Search;

    public abstract class BaseModule : NancyModule
    {
        private readonly IMappingEngine mappingEngine;

        public BaseModule(string modulePath, IMappingEngine mappingEngine) : base(modulePath)
        {
            this.mappingEngine = mappingEngine;
        }

        public Negotiator RespondWithValidationFailure(ModelValidationResult validationResult)
        {
            return Negotiate
                .WithStatusCode(HttpStatusCode.BadRequest)
                .WithModel(new
                {
                    ValidationErrors = validationResult.Errors.SelectMany(x => x.Value.Select(e => new { Property = x.Key, e.ErrorMessage }))
                });
        }

        protected TModel MapTo<TModel>(object entity)
        {
            return mappingEngine.Map<TModel>(entity);
        }

        protected dynamic MapToResultList<TEntity, TModel>(IPagedListResult<TEntity> sourceList)
        {
            if (sourceList == null)
                return null;

            var results = mappingEngine.Map<IEnumerable<TModel>>(sourceList.Results).ToList();

            return new PagedListResult<TModel>
            {
                Count = sourceList.Count,
                Pages = sourceList.Pages,
                Results = results
            };
        }
    }
}
