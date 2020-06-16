using System.Collections.Generic;

namespace sors.Data.Dto
{
    public class AccountForListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public DepartmentForListDto Department { get; set; }
        public IEnumerable<RoleForListDto> AccountRoles { get; set; }
    }
}