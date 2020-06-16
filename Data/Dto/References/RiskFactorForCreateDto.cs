using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sors.Data.Dto.References
{
    public class RiskFactorForCreateDto
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
    }
}
