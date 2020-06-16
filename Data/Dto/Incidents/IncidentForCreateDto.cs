using sors.Data.Entities.References;
using System;
using System.Collections.Generic;

namespace sors.Data.Dto.Incidents
{
    public class IncidentForCreateDto
    {
        public DateTime DateIncident { get; set; }
        public string Description { get; set; }
        public List<ResponsibleForListDto> Responsibles { get; set; }
        public List<DraftForListDto> Drafts { get; set; }
        public string Comment { get; set; }
        public string Status { get; set; }
        public int AuthorId { get; set; }
        public int IncidentTypeId { get; set; }
    }
}
