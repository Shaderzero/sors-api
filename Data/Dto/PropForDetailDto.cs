using System;

namespace sors.Data.Dto
{
    public class PropForDetailDto
    {
        public AccountForListDto Author { get; set; }
        public string Action { get; set; }
        public string Comment { get; set; }
        public DateTime DateCreate { get; set; }
    }
}
