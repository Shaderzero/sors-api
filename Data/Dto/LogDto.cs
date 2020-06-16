using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sors.Data.Dto
{
    public class LogDto
    {
        public int Id { get; set; }
        public AccountForListDto Account { get; set; }
        public string Action { get; set; }
        public string Info { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
