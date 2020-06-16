namespace sors.Helpers
{
    public class Params
    {
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        private int pageSize = 10;
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = (value > MaxPageSize) ? MaxPageSize : value; }
        }

        public int AccountId { get; set; }
        public int DepartmentId { get; set; }
        public string FilterColumn { get; set; }
        public string Filter { get; set; }
        public string Order { get; set; }
        public bool OrderAsc { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
    }
}
