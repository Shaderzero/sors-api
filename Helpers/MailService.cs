using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using sors.Authentication;
using sors.Data;
using sors.Data.Dto;
using sors.Data.Entities.Incidents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace sors.Helpers
{
    public class MailService : IMailService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly string _from;
        private readonly string _siteAddress;
        private readonly IUserService _userService;
        private SmtpClient _client;
        private readonly IConfiguration _config;

        public MailService(DataContext context, IConfiguration configuration, IUserService userService, IMapper mapper)
        {
            _from = configuration.GetValue<string>("Email:From");
            _siteAddress = configuration.GetValue<string>("SiteAddress");
            _mapper = mapper;
            _context = context;
            _userService = userService;
            _config = configuration;
            _client = CreateClient();
        }

        private SmtpClient CreateClient()
        {
            SmtpClient client = new SmtpClient();
            NetworkCredential nc = CredentialCache.DefaultNetworkCredentials;
            client.Host = _config.GetValue<string>("Email:Smtp:Host");
            client.Port = _config.GetValue<int>("Email:Smtp:Port");
            client.UseDefaultCredentials = true;
            client.Credentials = (System.Net.ICredentialsByHost)nc.GetCredential(
                        _config.GetValue<String>("Email:Smtp:Host"),
                        _config.GetValue<int>("Email:Smtp:Port"),
                        "Basic");
            return client;
        }

        public async Task<int> SendApprovalMail(Incident incident)
        {
            var accountsFromDb = await _context.Accounts
                .Where(a => _context.Responsibles.Any(r => r.DepartmentId == a.DepartmentId && r.IncidentId == incident.Id && r.Result == "watch"))
                .Where(a => _context.AccountRoles.Any(r => r.Role.Name.Equals("riskCoordinator") && r.AccountId == a.Id))
                .ToListAsync();
            var accountsRM = await _context.Accounts
                .Where(a => a.AccountRoles.Any(x => x.Role.Name.Equals("riskManager")))
                //.Where(a => _context.AccountRoles.Any(x => x.Role.Name.Equals("riskManager") && x.AccountId == a.Id))
                .ToListAsync();
            accountsFromDb.AddRange(accountsRM);
            if (accountsFromDb == null)
                return 0;
            var accounts = _mapper.Map<IEnumerable<AccountForListDto>>(accountsFromDb);
            var subject = "ApproveIncidentMailSubject";
            var subjectTD = await _context.TextDatas
                .Where(e => e.Name.Equals(subject) && e.Param.Equals("ru"))
                .FirstOrDefaultAsync();
            if (subjectTD != null)
            {
                subject = subjectTD.Value;
            }
            var text = "ApproveIncidentMailText";
            var textTD = await _context.TextDatas
                .Where(e => e.Name.Equals(text) && e.Param.Equals("ru"))
                .FirstOrDefaultAsync();
            if (textTD != null)
            {
                text = textTD.Value;
            }
            var link = "/incidents/" + incident.Id;
            var body = GenerateBody(text, link, "");
            List<string> recipients = new List<string>();
            foreach (var a in accounts)
            {
                recipients.Add(a.Email);
            }

            if (await Send(recipients, subject, body))
            {
                return 1;
            }
            else return 0;
            //var subject = "Для работы доступно рисковое событие";
            //var text = "Рисковое событие доступно для обработки. Вы можете назначить ответственных работников подразделения, " +
            //    "которые будут осуществлять его отслеживание и составлять план мероприятий";
            //var link = "/incidents/" + incident.Id;
            //var comment = "";

            //try
            //{
            //    using (var smtpClient = _client)
            //    {
            //        MailMessage mailMessage = new MailMessage();
            //        mailMessage.From = new MailAddress(_from);
            //        foreach (var a in accounts)
            //        {
            //            mailMessage.To.Add(new MailAddress(a.Email));
            //        }
            //        mailMessage.IsBodyHtml = true;
            //        mailMessage.SubjectEncoding = Encoding.UTF8;
            //        mailMessage.BodyEncoding = Encoding.UTF8;
            //        mailMessage.Subject = subject;
            //        mailMessage.Body = GenerateBody(text, link, comment);

            //        await smtpClient.SendMailAsync(mailMessage);
            //        return 1;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    return -1;
            //}
        }

        public async Task<int> SendCheckDraftMail(Draft draft, string comment)
        {
            var accountsFromDb = await _context.Accounts
                .Where(a => _context.AccountRoles.Any(r => r.Role.Name.Equals("riskCoordinator") && r.AccountId == a.Id) && a.DepartmentId == draft.DepartmentId)
                .ToListAsync();
            if (accountsFromDb == null)
                return 0;
            var accounts = _mapper.Map<IEnumerable<AccountForListDto>>(accountsFromDb);
            var subject = "CheckDraftMailSubject";
            var subjectTD = await _context.TextDatas
                .Where(e => e.Name.Equals(subject) && e.Param.Equals("ru"))
                .FirstOrDefaultAsync();
            if (subjectTD != null)
            {
                subject = subjectTD.Value;
            }
            var text = "CheckDraftMailText";
            var textTD = await _context.TextDatas
                .Where(e => e.Name.Equals(text) && e.Param.Equals("ru"))
                .FirstOrDefaultAsync();
            if (textTD != null)
            {
                text = textTD.Value;
            }
            var link = "/incidents/drafts/" + draft.Id;
            var body = GenerateBody(text, link, comment);
            List<string> recipients = new List<string>();
            foreach (var a in accounts)
            {
                recipients.Add(a.Email);
            }

            if (await Send(recipients, subject, body))
            {
                return 1;
            }
            else return 0;
        }

        public async Task<int> SendSignDraftMail(Draft draft, string comment)
        {
            var accountsFromDb = await _context.Accounts
                .Where(a => _context.AccountRoles.Any(r => r.Role.Name.Equals("riskManager") && r.AccountId == a.Id))
                .ToListAsync();
            if (accountsFromDb == null)
                return 0;
            var accounts = _mapper.Map<IEnumerable<AccountForListDto>>(accountsFromDb);
            var subject = "SignDraftMailSubject";
            var subjectTD = await _context.TextDatas
                .Where(e => e.Name.Equals(subject) && e.Param.Equals("ru"))
                .FirstOrDefaultAsync();
            if (subjectTD != null)
            {
                subject = subjectTD.Value;
            }
            var text = "SignDraftMailText";
            var textTD = await _context.TextDatas
                .Where(e => e.Name.Equals(text) && e.Param.Equals("ru"))
                .FirstOrDefaultAsync();
            if (textTD != null)
            {
                text = textTD.Value;
            }
            var link = "/incidents/drafts/" + draft.Id;
            var body = GenerateBody(text, link, comment);
            List<string> recipients = new List<string>();
            foreach (var a in accounts)
            {
                recipients.Add(a.Email);
            }

            if (await Send(recipients, subject, body))
            {
                return 1;
            }
            else return 0;
        }

        public async Task<int> SendRefineDraftMail(Draft draft, string comment)
        {
            var accountsFromDb = await _context.Accounts
                .Where(a => a.DepartmentId == draft.DepartmentId)
                .Where(a => _context.AccountRoles.Any(r => r.Role.Name.Equals("riskCoordinator") && r.AccountId == a.Id) || a.Id == draft.AuthorId)
                .ToListAsync();
            if (accountsFromDb == null)
                return 0;
            var accounts = _mapper.Map<IEnumerable<AccountForListDto>>(accountsFromDb);
            var subject = "RefineDraftMailSubject";
            var subjectTD = await _context.TextDatas
                .Where(e => e.Name.Equals(subject) && e.Param.Equals("ru"))
                .FirstOrDefaultAsync();
            if (subjectTD != null)
            {
                subject = subjectTD.Value;
            }
            var text = "RefineDraftMailText";
            var textTD = await _context.TextDatas
                .Where(e => e.Name.Equals(text) && e.Param.Equals("ru"))
                .FirstOrDefaultAsync();
            if (textTD != null)
            {
                text = textTD.Value;
            }
            var link = "/incidents/drafts/" + draft.Id;
            var body = GenerateBody(text, link, comment);
            List<string> recipients = new List<string>();
            foreach (var a in accounts)
            {
                recipients.Add(a.Email);
            }

            if (await Send(recipients, subject, body))
            {
                return 1;
            }
            else return 0;
        }

        public async Task<int> SendCloseDraftMail(Draft draft, string comment)
        {
            var accountsFromDb = await _context.Accounts
                .Where(a => a.DepartmentId == draft.DepartmentId)
                .Where(a => _context.AccountRoles.Any(r => r.Role.Name.Equals("riskCoordinator") && r.AccountId == a.Id) || a.Id == draft.AuthorId)
                .ToListAsync();
            if (accountsFromDb == null)
                return 0;
            var accounts = _mapper.Map<IEnumerable<AccountForListDto>>(accountsFromDb);
            var subject = "CloseDraftMailSubject";
            var subjectTD = await _context.TextDatas
                .Where(e => e.Name.Equals(subject) && e.Param.Equals("ru"))
                .FirstOrDefaultAsync();
            if (subjectTD != null)
            {
                subject = subjectTD.Value;
            }
            var text = "CloseDraftMailText";
            var textTD = await _context.TextDatas
                .Where(e => e.Name.Equals(text) && e.Param.Equals("ru"))
                .FirstOrDefaultAsync();
            if (textTD != null)
            {
                text = textTD.Value;
            }
            var link = "/incidents/drafts/" + draft.Id;
            var body = GenerateBody(text, link, comment);
            List<string> recipients = new List<string>();
            foreach (var a in accounts)
            {
                recipients.Add(a.Email);
            }

            if (await Send(recipients, subject, body))
            {
                return 1;
            }
            else return 0;
        }

        public async Task<int> SendCloseIncidentMail(Incident incident, string comment)
        {
            var accountsFromDb = await _context.Accounts
                .Where(a => _context.Responsibles.Any(r => r.IncidentId == incident.Id && r.DepartmentId == a.DepartmentId))
                .Where(a => _context.AccountRoles.Any(r => r.Role.Name.Equals("riskCoordinator") && r.AccountId == a.Id))
                .ToListAsync();
            var accountsFromDrafts = await _context.Accounts
                .Where(a => _context.IncidentDrafts.Any(x => x.IncidentId == incident.Id && x.Draft.AuthorId == a.Id))
                .ToListAsync();
            accountsFromDb.AddRange(accountsFromDrafts);
            if (accountsFromDb == null)
                return 0;
            var accounts = _mapper.Map<IEnumerable<AccountForListDto>>(accountsFromDb);
            var subject = "CloseIncidentMailSubject";
            var subjectTD = await _context.TextDatas
                .Where(e => e.Name.Equals(subject) && e.Param.Equals("ru"))
                .FirstOrDefaultAsync();
            if (subjectTD != null)
            {
                subject = subjectTD.Value;
            }
            var text = "CloseIncidentMailText";
            var textTD = await _context.TextDatas
                .Where(e => e.Name.Equals(text) && e.Param.Equals("ru"))
                .FirstOrDefaultAsync();
            if (textTD != null)
            {
                text = textTD.Value;
            }
            var link = "/incidents/" + incident.Id;
            var body = GenerateBody(text, link, comment);
            List<string> recipients = new List<string>();
            foreach (var a in accounts)
            {
                recipients.Add(a.Email);
            }

            if (await Send(recipients, subject, body))
            {
                return 1;
            }
            else return 0;
        }

        public async Task<int> SendRefineMail(Incident incident, string comment)
        {
            var accountsFromDb = await _context.Accounts
                .Where(a => _context.AccountRoles.Any(r => r.Role.Name.Equals("riskManager") && r.AccountId == a.Id))
                .ToListAsync();
            if (accountsFromDb == null)
                return 0;
            var accounts = _mapper.Map<IEnumerable<AccountForListDto>>(accountsFromDb);
            var subject = "RefineIncidentMailSubject";
            var subjectTD = await _context.TextDatas
                .Where(e => e.Name.Equals(subject) && e.Param.Equals("ru"))
                .FirstOrDefaultAsync();
            if (subjectTD != null)
            {
                subject = subjectTD.Value;
            }
            var text = "RefineIncidentMailText";
            var textTD = await _context.TextDatas
                .Where(e => e.Name.Equals(text) && e.Param.Equals("ru"))
                .FirstOrDefaultAsync();
            if (textTD != null)
            {
                text = textTD.Value;
            }
            var link = "/incidents/" + incident.Id;
            var body = GenerateBody(text, link, comment);
            List<string> recipients = new List<string>();
            foreach (var a in accounts)
            {
                recipients.Add(a.Email);
            }

            if (await Send(recipients, subject, body))
            {
                return 1;
            }
            else return 0;
            //var subject = "На доработку отправлено рисковое событие";
            //var text = "Рисковое событие направлено на доработку.";
            //var link = "/incidents/" + incident.Id;

            //try
            //{
            //    using (var smtpClient = _client)
            //    {
            //        MailMessage mailMessage = new MailMessage();
            //        mailMessage.From = new MailAddress(_from);
            //        foreach (var a in accounts)
            //        {
            //            mailMessage.To.Add(new MailAddress(a.Email));
            //        }
            //        mailMessage.IsBodyHtml = true;
            //        mailMessage.SubjectEncoding = Encoding.UTF8;
            //        mailMessage.BodyEncoding = Encoding.UTF8;
            //        mailMessage.Subject = subject;
            //        mailMessage.Body = GenerateBody(text, link, comment);

            //        await smtpClient.SendMailAsync(mailMessage);
            //        return 1;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    return -1;
            //}
        }

        public async Task<int> SendResignMail(Responsible responsible, int incidentId, string comment)
        {
            var responsibles = new List<Responsible>();
            responsibles.Add(responsible);
            return await SendResignMail(responsibles, incidentId, comment);
        }

        public async Task<int> SendResignMail(IEnumerable<Responsible> responsibles, int incidentId, string comment)
        {
            var accountsFromDb = await _context.Accounts
                .Where(a => a.Department.Responsibles.Any(x => responsibles.Contains(x)))
                .Where(a => _context.AccountRoles.Any(r => r.Role.Name.Equals("riskCoordinator") && r.AccountId == a.Id))
                .ToListAsync();
            if (accountsFromDb == null)
                return 0;
            var accounts = _mapper.Map<IEnumerable<AccountForListDto>>(accountsFromDb);
            var subject = "ResignIncidentMailSubject";
            var subjectTD = await _context.TextDatas
                .Where(e => e.Name.Equals(subject) && e.Param.Equals("ru"))
                .FirstOrDefaultAsync();
            if (subjectTD != null)
            {
                subject = subjectTD.Value;
            }
            var text = "ResignIncidentMailText";
            var textTD = await _context.TextDatas
                .Where(e => e.Name.Equals(text) && e.Param.Equals("ru"))
                .FirstOrDefaultAsync();
            if (textTD != null)
            {
                text = textTD.Value;
            }
            var link = "/incidents/" + incidentId;
            var body = GenerateBody(text, link, comment);
            List<string> recipients = new List<string>();
            foreach (var a in accounts)
            {
                recipients.Add(a.Email);
            }

            if (await Send(recipients, subject, body))
            {
                return 1;
            }
            else return 0;
        }

        public async Task<int> SendAssignMail(IEnumerable<ResponsibleAccount> responsibleAccounts, int incidentId)
        {
            var subject = "AssignIncidentMailSubject";
            var subjectTD = await _context.TextDatas
                .Where(e => e.Name.Equals(subject) && e.Param.Equals("ru"))
                .FirstOrDefaultAsync();
            if (subjectTD != null)
            {
                subject = subjectTD.Value;
            }
            var text = "AssignIncidentMailText";
            var textTD = await _context.TextDatas
                .Where(e => e.Name.Equals(text) && e.Param.Equals("ru"))
                .FirstOrDefaultAsync();
            if (textTD != null)
            {
                text = textTD.Value;
            }
            var link = "/incidents/" + incidentId;
            var body = GenerateBody(text, link, "");
            List<string> recipients = new List<string>();
            foreach (var item in responsibleAccounts)
            {
                var acc = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == item.AccountId);
                if (acc != null)
                {
                    var account = _mapper.Map<AccountForListDto>(acc);
                    recipients.Add(account.Email);
                }
            }
            if (recipients.Count > 0)
            {
                if (await Send(recipients, subject, body))
                {
                    return 1;
                }
            }
            return 0;
        }

        private async Task<Boolean> Send(List<string> recipients, string subject, string body)
        {
            try
            {
                using (var smtpClient = _client)
                {
                    MailMessage mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress(_from);
                    foreach (var a in recipients)
                    {
                        mailMessage.To.Add(new MailAddress(a));
                    }
                    mailMessage.IsBodyHtml = true;
                    mailMessage.SubjectEncoding = Encoding.UTF8;
                    mailMessage.BodyEncoding = Encoding.UTF8;
                    mailMessage.Subject = subject;
                    mailMessage.Body = body;

                    await smtpClient.SendMailAsync(mailMessage);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        private string GenerateBody(string text, string link, string comment)
        {
            string body = "<html>" +
                          "<h2>Система обработки рисковых событий</h2>" +
                          "<p>" + text + "</p>" +
                          "<p>" + _siteAddress + link + "</p>" +
                          "<p>" + comment + "</p>" +
                          "</html>";
            return body;
        }
    }
}
