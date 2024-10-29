using Microsoft.AspNetCore.ResponseCompression;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Suo.Autorization.Extentions;
using Suo.Autorization.SingleService.Extentions;
using Suo.Autorization.SingleService.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
});
builder.Services.AddRazorPages();
builder.AddDbContext();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = false;
    options.Cookie.IsEssential = true;
});
builder.Services.AddCurrentUserService();
builder.Services.AddForwarding();
builder.Services.AddIdentity();
var appsettings = builder.Services.GetApplicationSettings(builder.Configuration);
builder.Services.AddJwtAuthentication(appsettings); 
builder.Services.AddScoped<AppConfiguration>(options=> appsettings);
builder.Services.AddServerServices();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});

builder.Services.AddScoped<DatabaseSeederUser>();

#if DEBUG

builder.Services.AddServerLogger();
builder.Services.AddScoped<DatabaseSeederCreater>();
builder.Services.AddScoped<DatabaseSeederSysAdmin>();
builder.Services.AddScoped<DatabaseSeeder>();
#endif
builder.Services.AddDistributedMemoryCache();



//builder.AddServerLogger();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
#if DEBUG
app.MapGet("/api/admin/", async (context) =>
{
    try
    {
        string credentrials = "";
        using (var service = app.Services.CreateScope())
        {
            var seeder = service.ServiceProvider.GetRequiredService<DatabaseSeeder>();
            credentrials = await seeder.AddAdministrator("admin@admin.com", "wlad1051");
        }
        await context.Response.WriteAsync(credentrials);
    }
    catch (Exception e)
    {
        var result = JsonConvert.SerializeObject(e);
        Console.WriteLine(e);
        await context.Response.WriteAsync(result);
    }
    //return await Task.CompletedTask;
});
app.MapGet("/api/admin/{email}/{password}", async (context) =>
{
    try
    {
        var email = context.GetRouteValue("email") as string;
        var password = context.GetRouteValue("password") as string;
        string credentrials = "";
        using (var service = app.Services.CreateScope())
        {
            var seeder = service.ServiceProvider.GetRequiredService<DatabaseSeeder>();
            credentrials = await seeder.AddAdministrator(email, password);
        }
        await context.Response.WriteAsync(credentrials);
    }
    catch (Exception e)
    {
        var result = JsonConvert.SerializeObject(e);
        Console.WriteLine(e);
        await context.Response.WriteAsync(result);
    }    
    //return await Task.CompletedTask;
}); ;
#endif


#if DEBUG
app.MapGet("/api/user/", async (context) =>
{
    try
    {
        string credentrials = "";
        using (var service = app.Services.CreateScope())
        {
            var seeder = service.ServiceProvider.GetRequiredService<DatabaseSeederUser>();
            credentrials = await seeder.AddUser("user@user.com", "wlad1051", 4);
        }
        await context.Response.WriteAsync(credentrials);
    }
    catch (Exception e)
    {
        var result = JsonConvert.SerializeObject(e);
        Console.WriteLine(e);
        await context.Response.WriteAsync(result);
    }
    //return await Task.CompletedTask;
});
app.MapGet("/api/user/{email}/{password}/{userId}", async (context) =>
{
    try
    {
        var email = context.GetRouteValue("email") as string;
        var password = context.GetRouteValue("password") as string;
        int userid = int.Parse(context.GetRouteValue("userId") as string);//валидация???

        string credentrials = "";
        using (var service = app.Services.CreateScope())
        {
            var seeder = service.ServiceProvider.GetRequiredService<DatabaseSeederUser>();
            credentrials = await seeder.AddUser(email, password, userid);
        }
        await context.Response.WriteAsync(credentrials);
    }
    catch (Exception e)
    {
        var result = JsonConvert.SerializeObject(e);
        Console.WriteLine(e);
        await context.Response.WriteAsync(result);
    }
    //return await Task.CompletedTask;
}); ;
#endif

#if DEBUG
app.MapGet("/api/creater/", async (context) =>
{
    try
    {
        string credentrials = "";
        using (var service = app.Services.CreateScope())
        {
            var seeder = service.ServiceProvider.GetRequiredService<DatabaseSeederCreater>();
            credentrials = await seeder.AddCreater("creater@admin.com", "wlad1051");
        }
        await context.Response.WriteAsync(credentrials);
    }
    catch (Exception e)
    {
        var result = JsonConvert.SerializeObject(e);
        Console.WriteLine(e);
        await context.Response.WriteAsync(result);
    }
    //return await Task.CompletedTask;
});
app.MapGet("/api/creater/{email}/{password}", async (context) =>
{
    try
    {
        var email = context.GetRouteValue("email") as string;
        var password = context.GetRouteValue("password") as string;
        string credentrials = "";
        using (var service = app.Services.CreateScope())
        {
            var seeder = service.ServiceProvider.GetRequiredService<DatabaseSeederCreater>();
            credentrials = await seeder.AddCreater(email, password);
        }
        await context.Response.WriteAsync(credentrials);
    }
    catch (Exception e)
    {
        var result = JsonConvert.SerializeObject(e);
        Console.WriteLine(e);
        await context.Response.WriteAsync(result);
    }
    //return await Task.CompletedTask;
}); ;
#endif

#if DEBUG
app.MapGet("/api/sysadmin/", async (context) =>
{
    try
    {
        string credentrials = "";
        using (var service = app.Services.CreateScope())
        {
            var seeder = service.ServiceProvider.GetRequiredService<DatabaseSeederSysAdmin>();
            credentrials = await seeder.AddSysAdmin("testsys@sys.com", "12345678");
        }
        await context.Response.WriteAsync(credentrials);
    }
    catch (Exception e)
    {
        var result = JsonConvert.SerializeObject(e);
        Console.WriteLine(e);
        await context.Response.WriteAsync(result);
    }
    //return await Task.CompletedTask;
});
app.MapGet("/api/sysadmin/{email}/{password}", async (context) =>
{
    try
    {
        var email = context.GetRouteValue("email") as string;
        var password = context.GetRouteValue("password") as string;
        string credentrials = "";
        using (var service = app.Services.CreateScope())
        {
            var seeder = service.ServiceProvider.GetRequiredService<DatabaseSeederSysAdmin>();
            credentrials = await seeder.AddSysAdmin(email, password);
        }
        await context.Response.WriteAsync(credentrials);
    }
    catch (Exception e)
    {
        var result = JsonConvert.SerializeObject(e);
        Console.WriteLine(e);
        await context.Response.WriteAsync(result);
    }
    //return await Task.CompletedTask;
}); ;
#endif
app.UsePathBase("/auth");
app.UseSession();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.MapControllers();

app.Run();


