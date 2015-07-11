namespace Encore.ReportingService
{
    using System;
    using System.Configuration;
    using System.Net;
    using System.IO;
    using System.Net.Mail;
    using System.Collections;
    using Encore.Domain.Interfaces.Services;
    using Encore.Domain.Entities;
    using Encore.Domain.Interfaces.DataStore;
    using log4net;

    public class ReportCommunication : IAlertReportStatus
    {
        private readonly ILog log = LogManager.GetLogger(typeof(ReportCommunication));

        private readonly IRepositoryContext context;

        public ReportCommunication(IRepositoryContext context)
        {
            this.context = context;
        }

        public void ReportComplete(Guid resultId, Guid requestingUserId)
        {
            var systemUserRepo = context.GetRepository<SystemUser>();
            var user = systemUserRepo.Get(requestingUserId);

            if (user == null || string.IsNullOrEmpty(user.Email))
                return;

            var template = GetTemplate("template.txt");
            var from = ConfigurationManager.AppSettings["DefaultFrom"];
            const string subject = "Report Completed";

            // send message
            if (template != string.Empty)
            {
                var reportURL = string.Format("{0}/pages/reports/results/{1}", ConfigurationManager.AppSettings["ServiceUrl"], resultId);
                template = template.Replace("[link]", reportURL);
                template = template.Replace("[name]", user.Name);
            }

            SendEmail(from, user.Email, subject, template);
        }

        private bool SendEmail(string from, string to, string subject, string message)
        {
            try
            {
                var smtpServer = ConfigurationManager.AppSettings["SMTPServer"];

                if (!string.IsNullOrEmpty(smtpServer))
                {
                    var Mail = new MailMessage(from, to, subject, message) {IsBodyHtml = false};

                    var smtp = new SmtpClient(smtpServer);
                    smtp.Send(Mail);
                }

                return true;
            }
            catch(Exception ex)
            {
                log.Error(ex);
                return false;
            }
        }

        private string GetTemplate(string filename)
        {
            string template = string.Empty;

            var fullPathToFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

            if (File.Exists(fullPathToFile))
            {
                using (var sr = new StreamReader(fullPathToFile))
                {
                    template = sr.ReadToEnd();
                }
            }
            else
            {
                log.ErrorFormat("Could not load template {0}", fullPathToFile);
            }

            return template;
        }
    }
}