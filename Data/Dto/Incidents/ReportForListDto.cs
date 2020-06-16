using System;

namespace sors.Data.Dto.Incidents
{
    public class ReportForListDto
    {
        public int Id { get; set; }
        public int MeasureId { get; set; }
        public string Description { get; set; }
        public DateTime DateCreate { get; set; }
        public string Status { get; set; }
    }
}
