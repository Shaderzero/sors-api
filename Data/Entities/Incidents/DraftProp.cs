using System;

namespace sors.Data.Entities.Incidents
{
    public class DraftProp
    {
        public int Id { get; set; }
        public int DraftId { get; set; }
        public Draft Draft { get; set; }
        public DateTime DateCreate { get; set; }
        public int AuthorId { get; set; }
        public Account Author { get; set; }
        public string Action { get; set; }
        public string Comment { get; set; }
    }
}
