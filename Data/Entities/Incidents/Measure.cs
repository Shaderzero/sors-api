using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sors.Data.Entities.Incidents
{
    public class Measure
    {
        public int Id { get; set; }
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime DateCreate { get; set; }
        public int ResponsibleId { get; set; }
        public Responsible Responsible { get; set; }
        public string Description { get; set; }
        public string ExpectedResult { get; set; }
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? DeadLine { get; set; }
        public string DeadLineText { get; set; }
        public ICollection<Report> Reports { get; set; } = new List<Report>();
        public string Status { get; set; }
        public ICollection<MeasureProp> Props { get; set; } = new List<MeasureProp>();
    }
}
