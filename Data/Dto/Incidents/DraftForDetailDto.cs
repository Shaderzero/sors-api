using sors.Data.Entities.References;
using System;
using System.Collections.Generic;

namespace sors.Data.Dto.Incidents
{
    public class DraftForDetailDto
    {
        public int Id { get; set; }
        public DateTime DateCreate { get; set; }
        public string Description1 { get; set; }
        public string Description2 { get; set; }
        public string Description3 { get; set; }
        public string Description4 { get; set; }
        public string Description5 { get; set; }
        public AccountForListDto Author { get; set; }
        public DepartmentForListDto Department { get; set; }
        public string Status { get; set; }
        public List<PropForDetailDto> Props { get; set; }
        public IncidentType IncidentType { get; set; }

    }
}
