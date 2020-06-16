namespace sors.Data.Entities.Incidents
{
    public class IncidentDraft
    {
        public int IncidentId { get; set; }
        public Incident Incident { get; set; }
        public int DraftId { get; set; }
        public Draft Draft { get; set; }
    }
}
