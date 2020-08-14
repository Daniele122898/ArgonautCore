namespace ArgonautCore.Network.Health.Models
{
    /// <summary>
    /// Enum for the actual status of the service
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// Service works as intended
        /// </summary>
        Healthy,
        
        /// <summary>
        /// Service has completely failed
        /// </summary>
        Outage,
        
        
        /// <summary>
        /// Service has run into some errors but is still functional to some capacity
        /// </summary>
        PartialOutage
    }
}