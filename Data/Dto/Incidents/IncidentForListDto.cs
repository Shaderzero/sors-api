using sors.Data.Entities.References;
using System;
using System.Collections.Generic;

namespace sors.Data.Dto.Incidents
{
    public class IncidentForListDto
    {
        public int Id { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateIncident { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public List<ResponsibleForListDto> Responsibles { get; set; }
        public IncidentType IncidentType { get; set; }
    }
}
