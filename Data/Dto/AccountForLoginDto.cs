using System.Collections.Generic;

namespace sors.Data.Dto
{
    public class AccountForLoginDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DepartmentForListDto Department { get; set; }
        public IEnumerable<RoleForListDto> AccountRoles { get; set; }
    }
}