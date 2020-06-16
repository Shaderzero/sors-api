namespace sors.Data.Entities.Incidents
{
    public class ResponsibleAccount
    {
        public int AccountId { get; set; }
        public Account Account { get; set; }
        public int ResponsibleId { get; set; }
        public Responsible Responsible { get; set; }
    }
}
