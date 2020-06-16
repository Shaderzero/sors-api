namespace sors.Data.Dto
{
    public class DomainDepartmentForListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DepartmentForListDto Department { get; set; }
    }
}
