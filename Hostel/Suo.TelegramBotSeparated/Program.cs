using HostelDB.AlfaPruefungDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Suo.TelegramBotSeparated;
using Suo.TelegramBotSeparated.Services;
using Suo.TelegramBotSeparated.Services.MongoService;
using Suo.TelegramBotSeparated.Services.RabbitMqService;
using Suo.TelegramBotSeparated.Services.TelegramBotService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<HostelDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("HostelContext")));
builder.Services.AddTransient<TelegramUserService>();
builder.Services.AddTransient<LogApplicationService>();
builder.Services.AddTransient<AddToMongoDbService>();
builder.Services.AddTransient<RabbitService>();
builder.Services.AddTransient<TelegramBotSeparate>();
builder.Services.AddTransient<DutyForTomorowMesageGeneratorService>();
builder.Services.AddHostedService<ITelegramHostedService>();

var applicationSettingsConfiguration = builder.Configuration.GetSection(nameof(TelegramConfiguration));
builder.Services.Configure<TelegramConfiguration>(applicationSettingsConfiguration);
var appConfig = applicationSettingsConfiguration.Get<TelegramConfiguration>();
/*var appConfig = builder.Services.GetApplicationSettings(configuration);*/
builder.Services.AddSingleton<TelegramConfiguration>(e => appConfig);

builder.Services.AddSingleton<RabbitMQConnectionFactory>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
