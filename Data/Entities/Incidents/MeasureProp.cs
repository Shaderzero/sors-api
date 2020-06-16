using System;

namespace sors.Data.Entities.Incidents
{
    public class MeasureProp
    {
        public int Id { get; set; }
        public int MeasureId { get; set; }
        public Measure Measure { get; set; }
        public DateTime DateCreate { get; set; }
        public int AuthorId { get; set; }
        public Account Author { get; set; }
        public string Action { get; set; }
        public string Comment { get; set; }
    }
}
