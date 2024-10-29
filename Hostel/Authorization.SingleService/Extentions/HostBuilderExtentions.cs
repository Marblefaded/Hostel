using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using Ams.Pm.Wasm.Core.Auth.Identity;
using Suo.Autorization.Data.Models;
using Suo.Autorization.Data.Models.Responce;
using Suo.Autorization.Extentions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Suo.Autorization.SingleService.Extentions.Logs;
using Suo.Autorization.SingleService.Infrastructure.Auth;
using Suo.Autorization.SingleService.Infrastructure.Auth.Identity;
using Suo.Autorization.SingleService.Infrastructure.Interfaces;
using Suo.Autorization.SingleService.Infrastructure.Models;

namespace Suo.Autorization.SingleService.Extentions;

public static class HostBuilderExtensions
{
    internal static WebApplicationBuilder AddDbContext(this WebApplicationBuilder builder)
    {



        var connectionStringAuth = builder.Configuration.GetConnectionString("AuthContext");

        builder.Services.AddDbContext<ApplicationAuthDbContext>(options =>
            options.UseSqlServer(connectionStringAuth), ServiceLifetime.Transient);
        return builder;


        //builder.Services.AddDbContext<ApplicationAuthDbContext>(options =>
        //    options.UseMySql(connectionStringAuth, ServerVersion.AutoDetect(connectionStringAuth)), ServiceLifetime.Transient);
        //return builder;
    }


    internal static IServiceCollection AddServerLogger(this IServiceCollection builder)
    {


        builder.TryAddEnumerable(
            ServiceDescriptor.Singleton<ILoggerProvider, LoggerProvider>());
        builder.AddSingleton<IPushNotificationsQueue, PushNotificationsQueue>();
        builder.AddHostedService<PushNotificationsDequeuer>();



        return builder;
    }


    internal static IServiceCollection AddForwarding(this IServiceCollection services)
    {
        //var applicationSettingsConfiguration = configuration.GetSection(nameof(AppConfiguration));
        //var config = applicationSettingsConfiguration.GetUser<AppConfiguration>();
        //if (config.BehindSSLProxy)
        //{
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            //if (!string.IsNullOrWhiteSpace(config.ProxyIP))
            //{
            //var ipCheck = config.ProxyIP;
            //if (IPAddress.TryParse(ipCheck, out var proxyIP))
            //options.KnownProxies.Add(proxyIP);
            //else
            //Log.Logger.Warning("Invalid Proxy IP of {IpCheck}, Not Loaded", ipCheck);
            //}
        });

        //services.AddCors(options =>
        //{
        //    options.AddDefaultPolicy(
        //        builder =>
        //        {
        //            builder
        //                .AllowCredentials()
        //                .AllowAnyHeader()
        //                .AllowAnyMethod()
        //                .WithOrigins(config.ApplicationUrl.TrimEnd('/'));
        //        });
        //});
        //}

        return services;
    }
    internal static IServiceCollection AddCurrentUserService(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }

    private static AppConfiguration AddAppConfig(this IConfiguration configuration)
    {
        var applicationSettingsConfiguration = configuration.GetSection(nameof(AppConfiguration));
        return applicationSettingsConfiguration.Get<AppConfiguration>();
    }

    internal static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        services
            .AddIdentity<AmsUser, AmsRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationAuthDbContext>()
            .AddDefaultTokenProviders();
        services.AddScoped<IAccountService, AccountService>();
        return services;
    }

    internal static AppConfiguration GetApplicationSettings(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var applicationSettingsConfiguration = configuration.GetSection(nameof(AppConfiguration));
        services.Configure<AppConfiguration>(applicationSettingsConfiguration);
        return applicationSettingsConfiguration.Get<AppConfiguration>();
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
                    OnChallenge = context =>
                    {
                        try
                        {
                            context.HandleResponse();
                            if (!context.Response.HasStarted)
                            {
                                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                                //if (!context.Response.Headers.IsReadOnly)
                                //{
                                context.Response.ContentType = "application/json";

                                //}
                                var result = JsonConvert.SerializeObject("You are not Authorized.");

                                return context.Response.WriteAsync(result);
                            }

                            return Task.CompletedTask;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                    },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        context.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject("You are not authorized to access this resource.");
                        return context.Response.WriteAsync(result);
                    },
                };
            });
        services.AddAuthorization(options =>
        {
            // Here I stored necessary permissions/roles in a constant
            foreach (var prop in typeof(Permissions).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
            {
                var propertyValue = prop.GetValue(null);
                if (propertyValue is not null)
                {
                    options.AddPolicy(propertyValue.ToString(), policy => policy.RequireClaim(ApplicationClaimTypes.Permission, propertyValue.ToString()));
                }
            }
        });
        return services;
    }


    //internal static WebApplicationBuilder AddServerLogger(this WebApplicationBuilder builder)
    //{

    //    builder.Logging.ClearProviders().AddConfiguration();

    //    //services.AddLogging();
    //    builder.Logging.Services.TryAddEnumerable(
    //        ServiceDescriptor.Singleton<ILoggerProvider, LoggerProvider>());
    //    builder.Logging.AddConsole();
    //    builder.Services.AddSingleton<IPushNotificationsQueue, PushNotificationsQueue>();
    //    builder.Services.AddHostedService<PushNotificationsDequeuer>();

    //    //builder.Services.AddSingleton<IAMSLoggerAsync, AMSLogger>();


    //    return builder;
    //}
    internal static WebApplication AddCustomExceptionHandler(this WebApplication app)
    {

        app.UseExceptionHandler(exceptionHandlerApp =>
        {
            exceptionHandlerApp.Run(async context =>
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = System.Net.Mime.MediaTypeNames.Application.Json;

                var exceptionHandlerPathFeature =
                    context.Features.Get<IExceptionHandlerFeature>();
                var exceprionModel = new ResponceException(exceptionHandlerPathFeature.Error);
                if (exceprionModel.Exception is DbUpdateConcurrencyException)
                {
                    exceprionModel.IsDbUpdateConcurrencyException = true;
                }
                await context.Response.WriteAsync(JsonConvert.SerializeObject(exceprionModel));

            });
        });
        return app;
    }


    internal static IServiceCollection AddServerServices(this IServiceCollection services)
    {
        var service = typeof(IServerSideService);
        var types = service
            .Assembly
            .GetExportedTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Where(x => x.GetInterfaces().Any(x => x == typeof(IServerSideService)))
            .Select(t => new
            {
                Name = t.Name,
                Service = t.GetInterface($"I{t.Name}"),
                Implementation = t
            })
            .Where(t => t.Service != null).ToList();

        foreach (var type in types)
        {
            //if (type.Service.IsAssignableFrom(service))
            {
                services.AddScoped(type.Service, type.Implementation);
            }
        }
        //services.AddScoped<ISubtaskService,SubtaskService>();
        return services;
    }

    //internal static IServiceCollection AddServerRepositoryes(this IServiceCollection services)
    //{
    //    var service = typeof(IRepository);
    //    var types = service
    //        .Assembly
    //        .GetExportedTypes().Where(t => t.IsClass && !t.IsAbstract).Where(x => x.GetInterfaces()
    //            .Any(x => x == typeof(IRepository)))
    //        .Select(t => new
    //        {
    //            Implementation = t,
    //            Name = t.Name,
    //        })
    //        .ToList();
    //    //var types = service
    //    //    .Assembly
    //    //    .GetExportedTypes()
    //    //    .Where(t => t.IsClass && !t.IsAbstract)
    //    //    .Select(t => new
    //    //    {
    //    //        Implementation = t
    //    //    })
    //    //    .Where(t => t.Implementation != null).ToList();

    //    foreach (var type in types)
    //    {
    //        //if (type.Service.IsAssignableFrom(service))
    //        {
    //            services.AddScoped(type.Implementation);
    //        }
    //    }
    //    //services.AddScoped<ISubtaskService,SubtaskService>();
    //    return services;
    //}
}