using sors.Data.Entities.Incidents;
using System.Collections.Generic;
using sors.Data.Entities.Passports;

namespace sors.Data.Entities
{
    public class Account
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; }

        public ICollection<AccountRole> AccountRoles { get; set; } = new List<AccountRole>();
        public ICollection<ResponsibleAccount> ResponsibleAccounts { get; set; } = new List<ResponsibleAccount>();
        public ICollection<DraftProp> DraftProps { get; set; } = new List<DraftProp>();
        public ICollection<IncidentProp> IncidentProps { get; set; } = new List<IncidentProp>();
        public ICollection<ResponsibleProp> ResponsibleProps { get; set; } = new List<ResponsibleProp>();
        public ICollection<MeasureProp> MeasureProps { get; set; } = new List<MeasureProp>();
        public ICollection<ReportProp> ReportProps { get; set; } = new List<ReportProp>();
        public ICollection<Log> Logs { get; set; } = new List<Log>();

    }
}