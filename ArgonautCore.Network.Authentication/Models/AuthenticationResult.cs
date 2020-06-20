using System.Collections.Generic;
using System.Security.Claims;

namespace ArgonautCore.Network.Authentication.Models
{
    /// <summary>
    /// The custom auth check function will return this result
    /// </summary>
    public struct AuthenticationResult
    {
        /// <summary>
        /// The claims that shall be added to the authentication. This should have
        /// things like the userId etc that can then be used in the controllers that are protected
        /// by the authentication
        /// </summary>
        public List<Claim> Claims { get; set; }
        
        /// <summary>
        /// Whether or not the authentication was successful 
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// If <see cref="Success"/> is set to false this will have a reason on to why
        /// it failed. If not a default reason just replaces it.
        /// </summary>
        public string FailureReason { get; set; }
    }
}