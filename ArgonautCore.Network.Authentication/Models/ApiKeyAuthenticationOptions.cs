using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace ArgonautCore.Network.Authentication.Models
{
    
    /// <summary>
    /// 
    /// </summary>
    public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
    {
        /// <summary>
        /// Which default authentication scheme to use. This is vital for the handler to know which
        /// headers he should take over.
        /// </summary>
        public string DefaultScheme = "APIKey";
        
        /// <summary>
        /// The scheme in use 
        /// </summary>
        public string Scheme => DefaultScheme;
        
        /// <summary>
        /// Also the scheme in use...
        /// </summary>
        public string AuthenticationType => DefaultScheme;
        
        
        /// <summary>
        /// The Api Key Header value that has to be set. This is the KEY of the header which will hold the actual
        /// authentication token.
        /// </summary>
        public string ApiKeyHeaderName = "X-Api-Key";
        
        
        /// <summary>
        /// Whether or not the authentication handler should log. It's default log level is debug. 
        /// </summary>
        public bool EnableLogging = false;

        /// <summary>
        /// The custom function that will actually check if the provided key is valid. This is absolutely necessary to be defined
        /// otherwise the Handler will throw on construction.
        /// </summary>
        public Func<string, IServiceScopeFactory, Task<AuthenticationResult>> AuthCheckFunc;
    }
}