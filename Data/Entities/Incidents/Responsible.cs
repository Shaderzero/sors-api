using System.Collections.Generic;

namespace sors.Data.Entities.Incidents
{
    public class Responsible
    {
        public int Id { get; set; }
        public int IncidentId { get; set; }
        public Incident Incident { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        public string Result { get; set; }
        public ICollection<ResponsibleAccount> Accounts { get; set; } = new List<ResponsibleAccount>();
        public ICollection<Measure> Measures { get; set; } = new List<Measure>();

        public ICollection<ResponsibleProp> Props { get; set; } = new List<ResponsibleProp>();
    }
}
