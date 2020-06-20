# Network Custom Authentication Handlers
<a href="https://www.nuget.org/packages/ArgonautCore.Network.Authentication/">
    <img src="https://img.shields.io/nuget/vpre/ArgonautCore.Network.Authentication.svg?maxAge=2592000?style=plastic">
</a>

These are helper functions and classes for custom authentication with a custom Authentication token / method.

This mainly consists of a custom Api Key authentication handler that is customizable yet simple and easy to use.  

## Usage

```csharp
public void ConfigureServices(IServiceCollection services)
        {
            // [...]

            // Add authentication
            services.AddAuthentication()
                // Add custom Api key support
                .AddApiKeySupport("ArgonautKey", op =>
                {
                    op.DefaultScheme = "ArgonautKey"; // Set a default scheme same as scheme above
                    op.EnableLogging = true; // Whether to log or not. Logs to Debug.
                    op.ApiKeyHeaderName = "X-Argonaut"; // Set the header key to be used.

                    // This function is going to be called if the handler recognizes a proper 
                    // auth request with the specified header key.
                    op.AuthCheckFunc = (apiHeader, serviceFactory) =>
                    {
                        // Do your auth check in here. You also have access to DI via the service factory.
                        // ...
                        if (apiHeader != "Test123")
                        {
                            return Task.FromResult(new AuthenticationResult()
                            {
                                Success = false,
                                FailureReason = "Wrong api key"
                            });
                        }
                        
                        // Here you add your claims that will be available in the 
                        // controllers after authentication is successful.
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, "someUser"),
                            new Claim(ClaimTypes.NameIdentifier, "1231231231")
                        };

                        return Task.FromResult(new AuthenticationResult()
                        {
                            Success = true,
                            Claims = claims
                        });
                    };
                });

            // Don't forget to also add authorization with the proper scheme!
            services.AddAuthorization(op =>
            {
                op.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes("ArgonautKey")
                    .Build();
            });

            // [...]
        }

         public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // [...]
            
            // Make sure your app uses Authorization and Authentication
            app.UseAuthorization();
            app.UseAuthentication();

            // [...]
        }
```