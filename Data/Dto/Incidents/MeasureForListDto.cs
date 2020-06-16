using System;

namespace sors.Data.Dto.Incidents
{
    public class MeasureForListDto
    {
        public int Id { get; set; }
        public int ResponsibleId { get; set; }
        public DateTime DateCreate { get; set; }
        public string Description { get; set; }
        public string ExpectedResult { get; set; }
        public DateTime? DeadLine { get; set; }
        public string DeadLineText { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }
        public int AuthorId { get; set; }
    }
}
