using System.Collections.Generic;

namespace sors.Data.Dto
{
    public class DepartmentForListDto
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string ShortName { get; set; }
        public string Name { get; set; }
        public List<DomainDepartmentForListDto> DomainDepartments { get; set; }
    }
}