using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sors.Data.Entities
{
    public class Log
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
        public string Action { get; set; }
        public string Info { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
