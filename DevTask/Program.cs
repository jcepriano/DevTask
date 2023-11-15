using DevTask.DataAccess;
using DevTask.Models;
using DevTask.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using Umbraco.Core.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<Repositories>();
builder.Services.AddHttpClient("GitHubApi", c => c.BaseAddress = new Uri("https://api.github.com/"));
builder.Services.AddDbContext<DevTaskContext>(
    options =>
        options
            .UseNpgsql(
                builder.Configuration["DEVTASK_DBCONNECTIONSTRING"]
                    ?? throw new InvalidOperationException(
                        "Connection string 'DevTaskDB' not found."
                    )
            )
            .UseSnakeCaseNamingConvention()
);
builder.Services.AddScoped<IUserRepository>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(options =>
{
    options.ClientId = "";
    options.ClientSecret = "";
});

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
