using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace sors.Data.Entities.References
{
    public class TextData
    {
        public int Id { get; set; }
        [MaxLength(255)]
        public string Name { get; set; }
        [MaxLength(1000)]
        public string Value { get; set; }
        [MaxLength(10)]
        public string Param { get; set; }
    }
}
