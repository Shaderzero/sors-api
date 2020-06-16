using System.Collections.Generic;

namespace sors.Data.Dto
{
    public class AccountForCreateDto
    {
        public string Name { get; set; }
        //public string Fullname { get; set; }
        //public string Email { get; set; }
        public DepartmentForListDto Department { get; set; }
        public List<RoleForListDto> AccountRoles { get; set; }
    }
}
