using System.Collections.Generic;

namespace sors.Data.Dto.Incidents
{
    public class ResponsibleForDetailDto
    {
        public int Id { get; set; }
        public DepartmentForListDto Department { get; set; }
        public string Result { get; set; }
        public List<AccountForListDto> Accounts { get; set; }
        public List<MeasureForListDto> Measures { get; set; }
    }
}
