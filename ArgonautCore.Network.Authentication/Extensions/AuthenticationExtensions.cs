using System;
using ArgonautCore.Network.Authentication.Models;
using Microsoft.AspNetCore.Authentication;

namespace ArgonautCore.Network.Authentication.Extensions
{
    /// <summary>
    /// Authentication Extension methods
    /// </summary>
    public static class AuthenticationExtensions
    {
        /// <summary>
        /// This cann be added to the startup.cs file to add the custom api key authentication. Make sure to properly set op the options object!
        /// Especially the authentication check function has to be defined!
        /// </summary>
        /// <param name="authenticationBuilder"></param>
        /// <param name="options"></param>
        /// <param name="defaultScheme"></param>
        /// <returns></returns>
        public static AuthenticationBuilder AddApiKeySupport(this AuthenticationBuilder authenticationBuilder, Action<ApiKeyAuthenticationOptions> options, string defaultScheme = "APIKey")
        {
            return authenticationBuilder.AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(defaultScheme, options);
        }
    }
}