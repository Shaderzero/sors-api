using sors.Data.Entities.References;
using System;
using System.Collections.Generic;

namespace sors.Data.Dto.Incidents
{
    public class IncidentForDetailDto
    {
        public int Id { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateIncident { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public List<DraftForDetailDto> Drafts { get; set; }
        public List<ResponsibleForDetailDto> Responsibles { get; set; }
        public IncidentType IncidentType { get; set; }
    }
}
