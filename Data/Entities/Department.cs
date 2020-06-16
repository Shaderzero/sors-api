using sors.Data.Entities.Incidents;
using System.Collections.Generic;
using sors.Data.Entities.Passports;

namespace sors.Data.Entities
{
    public class Department
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public ICollection<DomainDepartment> DomainDepartments { get; set; } = new List<DomainDepartment>();
        public ICollection<Account> Accounts { get; set; } = new List<Account>();
        public ICollection<Draft> Drafts { get; set; } = new List<Draft>();
        public ICollection<Responsible> Responsibles { get; set; } = new List<Responsible>();
    }
}