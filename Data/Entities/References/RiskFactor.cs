using sors.Data.Entities.Passports;
using System.Collections.Generic;

namespace sors.Data.Entities.References
{
    public class RiskFactor
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public RiskFactor Parent { get; set; }
        public ICollection<RiskFactor> Children { get; set; }
    }
}
