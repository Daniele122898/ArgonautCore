using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using ArgonautCore.Network.Authentication.Extensions;
using ArgonautCore.Network.Authentication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace ArgonautCore.Network.Authentication.Test
{
    public class Startup
    {
        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Title = "Authentication Test",
                    Version = "v1",
                    Description = "Authentication Test"
                });
                
                c.AddSecurityDefinition("X-Argonaut", new OpenApiSecurityScheme()
                {
                    Description = "Used for Token",
                    Name = "X-Argonaut",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "X-Argonaut"
                });
                
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme()
                        {
                            Reference = new OpenApiReference()
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "X-Argonaut"
                            },
                            Scheme = "oauth2",
                            Name = "X-Argonaut",
                            In = ParameterLocation.Header
                        }, 
                        new List<string>()
                    }
                });
                
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                
            });
            
            this.ConfigureServices(services);
        }
        
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddRouting(op => op.LowercaseUrls = true);

            // Add authentication
            services.AddAuthentication()
                // Add custom Api key support
                .AddApiKeySupport("ArgonautKey", op =>
                {
                    op.DefaultScheme = "ArgonautKey"; // Set a default scheme
                    op.EnableLogging = true; // Whether to log or not. Logs to Debug.
                    op.ApiKeyHeaderName = "X-Argonaut"; // Set the header key to be used.

                    // This function is going to be called if the handler recognizes a proper 
                    // auth request with the specified header key.
                    op.AuthCheckFunc = (apiHeader, serviceFactory) =>
                    {
                        // Do your testing in here. You also have access to DI via the service factory.
                        // ...
                        if (apiHeader != "Test123")
                        {
                            return Task.FromResult(new AuthenticationResult()
                            {
                                Success = false,
                                FailureReason = "Wrong api key"
                            });
                        }
                        
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

            services.AddCors();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                
                // Enable middleware to serve generated swagger as a json endpoint
                app.UseSwagger();
                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
                // specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CDN.NET");
                });
            }

            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            
            app.UseRouting();
            
            // Make sure your app uses Authorization and Authentication
            app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}