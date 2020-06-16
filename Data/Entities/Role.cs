using System.Collections.Generic;

namespace sors.Data.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<AccountRole> AccountRoles { get; set; }
    }
}