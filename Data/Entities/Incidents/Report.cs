using System;
using System.Collections.Generic;

namespace sors.Data.Entities.Incidents
{
    public class Report
    {
        public int Id { get; set; }
        public int MeasureId { get; set; }
        public Measure Measure { get; set; }
        public string Description { get; set; }
        public DateTime DateCreate { get; set; }
        public string Status { get; set; }

        public ICollection<ReportProp> Props { get; set; } = new List<ReportProp>();
    }
}
