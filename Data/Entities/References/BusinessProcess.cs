using System.Collections.Generic;

namespace sors.Data.Entities.References
{
    public class BusinessProcess
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public BusinessProcess Parent { get; set; }
        public ICollection<BusinessProcess> Children { get; set; }
    }
}
