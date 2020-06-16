using sors.Data.Entities.Incidents;
using sors.Data.Entities.Passports;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace sors.Data.Entities.References
{
    public class IncidentType
    {
        public int Id { get; set; }
        [MaxLength(1000)]
        public string Name { get; set; }

        public ICollection<Draft> Drafts { get; set; } = new List<Draft>();
        public ICollection<Incident> Incidents { get; set; } = new List<Incident>();
    }
}
