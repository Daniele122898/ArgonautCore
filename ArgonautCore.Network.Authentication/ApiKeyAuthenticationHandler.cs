using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ArgonautCore.Network.Authentication.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ArgonautCore.Network.Authentication
{
    /// <summary>
    /// Custom Api Key authentication handler.
    /// </summary>
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        private readonly IServiceScopeFactory _factory;
        private const string _PROBLEM_DETAILS_CONTENT_TYPE = "application/problem+json";
        
        private readonly string _apiKeyHeaderName;
        private readonly ILogger _log;
        private readonly bool _enableLogging;
        private readonly Func<string, IServiceScopeFactory, Task<AuthenticationResult>> _authFunc;

        /// <summary>
        /// Create a new instance. This should be left to the DI to do :)
        /// </summary>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        /// <param name="encoder"></param>
        /// <param name="clock"></param>
        /// <param name="factory"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<ApiKeyAuthenticationOptions> options, 
            ILoggerFactory logger, 
            UrlEncoder encoder, 
            ISystemClock clock,
            IServiceScopeFactory factory) : base(options, logger, encoder, clock)
        {
            _factory = factory;
            _apiKeyHeaderName = options.CurrentValue.ApiKeyHeaderName;

            _authFunc = options.CurrentValue.AuthCheckFunc;
            if (_authFunc == null)
                throw new ArgumentNullException($"{nameof(options.CurrentValue.AuthCheckFunc)} cannot be null");

            _enableLogging = options.CurrentValue.EnableLogging;
            _log = logger.CreateLogger(typeof(ApiKeyAuthenticationHandler));
        }

        /// <summary>
        /// This method handles the authentication if the proper scheme is being used.
        /// </summary>
        /// <returns></returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!this.Request.Headers.TryGetValue(_apiKeyHeaderName, out var apiKeyHeaderValues))
            {
                return AuthenticateResult.NoResult();
            }
            
            if (apiKeyHeaderValues.Count == 0) 
                return AuthenticateResult.NoResult();

            var keyHeader = apiKeyHeaderValues[0];
            if (string.IsNullOrWhiteSpace(keyHeader))
                return AuthenticateResult.NoResult();

            var res = await _authFunc(keyHeader, _factory).ConfigureAwait(false);
            if (!res.Success)
                return AuthenticateResult.Fail(string.IsNullOrWhiteSpace(res.FailureReason) ? "Invalid authentication key." : res.FailureReason);

            var claims = res.Claims;
            var identity = new ClaimsIdentity(claims, Options.AuthenticationType);
            var identities = new List<ClaimsIdentity>{identity};
            var principal = new ClaimsPrincipal(identities);
            var ticket = new AuthenticationTicket(principal, Options.Scheme);
            
            return AuthenticateResult.Success(ticket);
        }
        
        /// <summary>
        /// If the authentication fails
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = 401;
            Response.ContentType = _PROBLEM_DETAILS_CONTENT_TYPE;
            
            if (_enableLogging)
                _log.LogDebug($"[{this.Request.Method}] {(this.Request.Path.Value ?? "/")} was not authorized.");

            return Task.CompletedTask;
        }

        /// <summary>
        /// If this action is forbidden.
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = 403;
            Response.ContentType = _PROBLEM_DETAILS_CONTENT_TYPE;
            
            if (_enableLogging)
                _log.LogDebug($"[{this.Request.Method}] {(this.Request.Path.Value ?? "/")} was forbidden.");

            return Task.CompletedTask;
        }
    }
}