using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using sors.Data.Entities.Passports;
using sors.Data.Entities.References;

namespace sors.Data.Entities.Incidents
{
    public class Incident
    {
        public int Id { get; set; }
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime DateCreate { get; set; }
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime DateIncident { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public int? IncidentTypeId { get; set; }
        public IncidentType IncidentType { get; set; }
        public ICollection<Responsible> Responsibles { get; set; } = new List<Responsible>();
        public ICollection<IncidentDraft> Drafts { get; set; } = new List<IncidentDraft>();
        public ICollection<IncidentProp> Props { get; set; } = new List<IncidentProp>();
    }
}
