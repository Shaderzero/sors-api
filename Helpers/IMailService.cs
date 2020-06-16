using sors.Data.Entities.Incidents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sors.Helpers
{
    public interface IMailService
    {
        public Task<int> SendApprovalMail(Incident incident);
        public Task<int> SendRefineMail(Incident incident, string comment);
        public Task<int> SendResignMail(Responsible responsible, int incidentId, string comment);
        public Task<int> SendResignMail(IEnumerable<Responsible> responsibles, int incidentId, string comment);
        public Task<int> SendAssignMail(IEnumerable<ResponsibleAccount> responsibleAccounts, int incidentId);
        public Task<int> SendCheckDraftMail(Draft draft, string comment);
        public Task<int> SendSignDraftMail(Draft draft, string comment);
        public Task<int> SendRefineDraftMail(Draft draft, string comment);
        public Task<int> SendCloseDraftMail(Draft draft, string comment);
        public Task<int> SendCloseIncidentMail(Incident incident, string comment);
    }
}
