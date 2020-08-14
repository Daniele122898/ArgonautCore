namespace ArgonautCore.Network.Health.Models
{
    /// <summary>
    /// Health status to be sent back to the Status Service API
    /// </summary>
    /// <remarks>
    /// Many of these values can be overwritten by the Status API. Such as Identifier and Description and status.
    /// These values should be set anyway though since the configuration step does not REQUIRE to override values.
    /// Also internal errors and statuses can only be communicated via the service itself. The Status API can only infer
    /// errors via HTTP errors.
    /// </remarks>
    public class HealthStatus
    {
        /// <summary>
        /// Identifier for this service
        /// </summary>
        /// <remarks>ATTENTION: Must be unique</remarks>
        public string Identifier { get; set; }

        /// <summary>
        /// Status of this service
        /// </summary>
        /// <remarks>In case of outage this will obviously be set by the Status API. But services have the
        /// ability to make self-checks and report their status this way</remarks>
        public Status Status { get; set; }
        
        /// <summary>
        /// Short description of what this service does. Not required.
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// If an error was detected by the service self-check.
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        public HealthStatus(string identifier, Status status)
        {
            this.Identifier = identifier;
            this.Status = status;
        }
    }
}