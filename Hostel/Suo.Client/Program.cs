using HostelDB.AlfaPruefungDb;
using HostelDB.DbRepository;
using Suo.Client.Controllers;
using Suo.Client.Data.Models.Services;
using Suo.Client.Data.Services;
using Suo.Client.Extentions;
using Suo.Client.Models;
using Microsoft.EntityFrameworkCore;
using NPOIPdfEngine;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Globalization;
using Suo.Client.Data.RabbitMqService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<HttpAuthenticationService>();
string connection = builder.Configuration.GetConnectionString("HostelContext");
builder.Services.AddDbContext<HostelDbContext>(options => options.UseSqlServer(connection));
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<HistoryService>();
builder.Services.AddTransient<UserService>();
builder.Services.AddTransient<RabbitService>();
builder.AddAppConfig();
builder.Services.AddScoped<Engine>();
builder.Services.AddServerLogger();
var app = builder.Build();
Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

var conf = app.Services.GetRequiredService<AppConfiguration>();
AppConfigGlobals.IdentityServer = conf.IdentityServer;
AppConfigGlobals.Secret = conf.Secret;

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();