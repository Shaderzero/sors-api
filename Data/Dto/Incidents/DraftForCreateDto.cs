namespace sors.Data.Dto.Incidents
{
    public class DraftForCreateDto
    {
        public string Description1 { get; set; }
        public string Description2 { get; set; }
        public string Description3 { get; set; }
        public string Description4 { get; set; }
        public string Description5 { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }
        public int AuthorId { get; set; }
        public int DepartmentId { get; set; }
        public int IncidentTypeId { get; set; }
    }
}
