using DevTask.DataAccess;
using DevTask.HelperServices;
using DevTask.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions()
{
    Args = args,
    ContentRootPath = "/app/out",
    WebRootPath = "wwwroot",
});
//var builder = WebApplication.CreateBuilder(args);
//string DEVELOPERDASHBOARD_DBCONNECTIONSTRING = $"Server={Environment.GetEnvironmentVariable("PGHOST")};Database={Environment.GetEnvironmentVariable("DATABASE_URL")};Port={Environment.GetEnvironmentVariable("PGPORT")};Username={Environment.GetEnvironmentVariable("POSTGRES_USER")};Password={Environment.GetEnvironmentVariable("PGPASSWORD")}";


builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
    .AddCookie()
    .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
    {
        options.ClientId = builder.Configuration["GoogleKeys:ClientId"];
        options.ClientSecret = builder.Configuration["GoogleKeys:ClientSecret"];
    });

builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<Repositories>();
builder.Services.AddHttpClient("GitHubApi", c => c.BaseAddress = new Uri("https://api.github.com/"));
builder.Services.AddDbContext<DevTaskContext>(
    options =>
        options
            .UseNpgsql(builder.Configuration["DEVTASK_DBCONNECTIONSTRING"]
                    ?? throw new InvalidOperationException(
                            "Connection String 'MYDBNOTFOUND' not found"
                            )
                    )
                    .UseSnakeCaseNamingConvention()
                    );
var app = builder.Build();
//builder.Configuration["DEVTASK_DBCONNECTIONSTRING"]
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DevTaskContext>();
    dbContext.Database.Migrate();
}

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
//