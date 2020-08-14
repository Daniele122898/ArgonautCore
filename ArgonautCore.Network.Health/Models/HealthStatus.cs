namespace ArgonautCore.Network.Health.Models
{
    public class HealthStatus
    {
        public string Identifier { get; set; }

        public Status Status { get; set; }
        
        public string Description { get; set; }
        
        public string Error { get; set; }

        public HealthStatus(string identifier, Status status)
        {
            this.Identifier = identifier;
            this.Status = status;
        }
    }
}