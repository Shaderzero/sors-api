using sors.Data.Entities.References;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sors.Data.Entities.Incidents
{
    public class Draft
    {
        public int Id { get; set; }
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime DateCreate { get; set; }
        public string Description1 { get; set; }
        public string Description2 { get; set; }
        public string Description3 { get; set; }
        public string Description4 { get; set; }
        public string Description5 { get; set; }
        public int? IncidentTypeId { get; set; }
        public IncidentType IncidentType { get; set; }
        public int AuthorId { get; set; }
        public Account Author { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        public string Status { get; set; }
        public ICollection<IncidentDraft> IncidentDrafts { get; set; }

        public ICollection<DraftProp> Props { get; set; } = new List<DraftProp>();

    }
}
