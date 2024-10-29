using HostelDB.AlfaPruefungDb;
using MatBlazor;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using MudBlazor.Services;
using NPOIPdfEngine;
using Suo.Admin.Data.RabbitMqService;
using Suo.Admin.Data.Service;
using Suo.Admin.Extentions;
using System.Globalization;

namespace Suo.Admin.Web
{
    public class Startup
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _hostingEnvironment = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            /*var builder = WebApplication.CreateBuilder();*/

            services.AddControllersWithViews();
            services.AddMvc(options => options.EnableEndpointRouting = false);
            services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;

                config.SnackbarConfiguration.ShowCloseIcon = true;
                config.SnackbarConfiguration.VisibleStateDuration = 5000;
                config.SnackbarConfiguration.HideTransitionDuration = 500;
                config.SnackbarConfiguration.ShowTransitionDuration = 500;
                config.SnackbarConfiguration.SnackbarVariant = Variant.Outlined;
            });
            services.AddScoped<MudThemeProvider>();
            services.AddMatBlazor();
            services.AddSignalR(e =>
            {
                e.MaximumReceiveMessageSize = 102400000;
            });
            services.AddRazorPages();
            services.AddServerSideBlazor();

            Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");

            services.AddAppConfig(Configuration);
            services.AddLocalStorage();
            services.AddServerAuthConfig();
            Blazored.LocalStorage.ServiceCollectionExtensions.AddBlazoredLocalStorage(services);
            services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });
            services.AddControllersWithViews();
            services.AddHttpContextAccessor();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHttpClient();
            services.AddScoped(r =>
            {
                var client = new HttpClient(new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator //()=> { true}
                });
                return client;
            });

            services.AddAuthorization(config =>
            {
                config.AddPolicy("AllUserRoles", policy => policy.RequireRole("Rolle Administrator", "Rolle PowerUserL2", "Rolle PowerUser", "Rolle User", "Rolle FiBu", "Rolle Berater", "Datenschutzbeauftragter"));
            });

            //services.AddDbContext<HostelDbSecond>(options => options.UseMySql(Configuration.GetConnectionString("HostelContext"), ServerVersion.AutoDetect(Configuration.GetConnectionString("HostelContext"))));
            //services.AddDbContext<HostelDbContext>(options => options.UseMySql(Configuration.GetConnectionString("HostelContext"), ServerVersion.AutoDetect(Configuration.GetConnectionString("HostelContext"))));

            services.AddDbContext<HostelDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("HostelContext")));
            services.AddDbContext<HostelDbSecond>(options => options.UseSqlServer(Configuration.GetConnectionString("HostelContext")));
            services.AddLogging();
            services.AddServerLogger();
            services.AddScoped<AppVersionService>();
            services.AddScoped<AspNetUserManagment>();
            services.AddScoped<Engine>();
            services.AddScoped<UserService>();
            services.AddScoped<ClaimTemplateService>();
            services.AddScoped<ClaimService>();
            services.AddScoped<PostService>();
            services.AddScoped<LogApplicationService>();
            services.AddScoped<UserRoomService>();
            services.AddScoped<RoomService>();
            services.AddScoped<DutyService>();
            services.AddScoped<DutyOrderService>();
            services.AddScoped<AmsUserService>();
            services.AddScoped<TelegramUserService>();
            services.AddScoped<RabbitService>();
            
            services.AddTransient<TelegramUserService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UsePathBase("/backend");

            app.UseStaticFiles();

            app.UseRouting();
            app.UseRequestLocalization();

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {


                endpoints.MapControllers();
                endpoints.MapControllerRoute(
            name: "default",
            pattern: "api/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });

        }
    }
}
