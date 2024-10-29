using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Net;
using System.Security.Claims;
using System.Text;
using Suo.Client.Extentions.Logs;

namespace Suo.Client.Extentions;

public static class HostBuilderExtentions
{
    internal static AppConfiguration GetApplicationSettings(
     this IServiceCollection services,
     IConfiguration configuration)
    {
        var applicationSettingsConfiguration = configuration.GetSection(nameof(AppConfiguration));
        services.Configure<AppConfiguration>(applicationSettingsConfiguration);
        return applicationSettingsConfiguration.Get<AppConfiguration>();
    }

    internal static IServiceCollection AddServerLogger(this IServiceCollection builder)
    {


        builder.TryAddEnumerable(
            ServiceDescriptor.Singleton<ILoggerProvider, LoggerProvider>());
        builder.AddSingleton<IPushNotificationsQueue, PushNotificationsQueue>();
        builder.AddHostedService<PushNotificationsDequeuer>();



        return builder;
    }

    internal static WebApplicationBuilder AddAppConfig(this WebApplicationBuilder builder)
    {

        var appConfig = builder.Services.GetApplicationSettings(builder.Configuration);
        builder.Services.AddTransient<AppConfiguration>(e => appConfig);
        builder.Services.AddJwtAuthentication(appConfig);

        return builder;
    }

    internal static IServiceCollection AddJwtAuthentication(
           this IServiceCollection services, AppConfiguration config)
    {
        //TODO Add AppSettings
        var key = Encoding.UTF8.GetBytes(config.Secret);
        services
            .AddAuthentication(authentication =>
            {
                authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(async bearer =>
            {
                bearer.RequireHttpsMetadata = false;
                bearer.SaveToken = true;
                bearer.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RoleClaimType = ClaimTypes.Role,
                    ClockSkew = TimeSpan.Zero
                };

                var localizer = "";

                bearer.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        //var path = context.HttpContext.Request.Path;
                        //if (!string.IsNullOrEmpty(accessToken) &&
                        //(path.StartsWithSegments(ApplicationConstants.SignalR.HubUrl)))
                        //{
                        //    context.Token = accessToken;
                        //}
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = c =>
                    {
                        try
                        {
                            if (c.Exception is SecurityTokenExpiredException)
                            {


                                if (!c.Response.HasStarted)
                                {
                                    c.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                    c.Response.ContentType = "application/json";
                                    var result = JsonConvert.SerializeObject("Authentication Failed.");
                                    //Console.WriteLine("response");

                                    return c.Response.WriteAsync(result);
                                }
                                else
                                {
                                    return Task.CompletedTask;
                                }
                            }
                            else
                            {
#if DEBUG
                                //c.NoResult();



                                if (!c.Response.HasStarted)
                                {
                                    c.Response.ContentType = "text/plain";

                                    c.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                    return c.Response.WriteAsync(c.Exception.ToString());
                                }
                                else
                                {

                                    return Task.CompletedTask;

                                }

#else

                                c.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                c.Response.ContentType = "application/json";
                                var result = JsonConvert.SerializeObject("Authentication Failed.");
                                return c.Response.WriteAsync(result);
#endif
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                    },
                    //OnChallenge = context =>
                    //{
                    //    try
                    //    {
                    //        context.HandleResponse();
                    //        if (!context.Response.HasStarted)
                    //        {
                    //            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                    //            //if (!context.Response.Headers.IsReadOnly)
                    //            //{
                    //            context.Response.ContentType = "application/json";

                    //            //}
                    //            var result = JsonConvert.SerializeObject("You are not Authorized.");

                    //            return context.Response.WriteAsync(result);
                    //        }

                    //        return Task.CompletedTask;
                    //    }
                    //    catch (Exception e)
                    //    {
                    //        Console.WriteLine(e);
                    //        throw;
                    //    }
                    //},
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        context.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject("You are not authorized to access this resource.");

                        return context.Response.WriteAsync(result);

                    },
                };
            }).AddCookie();
        services.AddAuthorization(options =>
        {
            // Here I stored necessary permissions/roles in a constant
            //foreach (var prop in typeof(Permissions).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
            //{
            //    var propertyValue = prop.GetValue(null);
            //    if (propertyValue is not null)
            //    {
            //        options.AddPolicy(propertyValue.ToString(), policy => policy.RequireClaim(ApplicationClaimTypes.Permission, propertyValue.ToString()));
            //    }
            //}
        });
        return services;
    }
}