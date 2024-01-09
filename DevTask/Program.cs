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
    options.CallbackPath = "/signin-google";
});

builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<Repositories>();
builder.Services.AddHttpClient("GitHubApi", c => c.BaseAddress = new Uri("https://api.github.com/"));
builder.Services.AddDbContext<DevTaskContext>(
    options =>
        options
            .UseNpgsql(ConnectionHelper.getConnectionString()
                    ?? throw new InvalidOperationException(
                            "Connection String 'MYDBNOTFOUND' not found"
                            )
                    )
                    .UseSnakeCaseNamingConvention()
                    );
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DevTaskContext>();
    dbContext.Database.Migrate();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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