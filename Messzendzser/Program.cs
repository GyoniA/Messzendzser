using Messzendzser.Model.DB;
using Messzendzser.Model.DB.Models;
using Messzendzser.Voip;
using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

new Thread(() => {
    try
    {
        //LumiSoft.Net.SIP.Proxy.SIP_ProxyCore proxyCore = new LumiSoft.Net.SIP.Proxy.SIP_ProxyCore()
        Thread.CurrentThread.Name = "Voip Thread";
        //var voipServer = new VoipServer(5060);
        var sipSerer = new SIPServer();
        sipSerer.Start();
    }
    catch(Exception ex)
    {
        Console.WriteLine($"Could not start Voip server: {ex.Message}");
    }
}).Start();


// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddAuthentication().AddCookie(options => {
    options.Cookie.Name = "new-user-token";
    options.Cookie.HttpOnly = false;
    options.Cookie.SameSite = SameSiteMode.None;
    }
);

// Add DbContext service
builder.Services.AddDbContext<IDataSource,MySQLDbConnection>();

builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<MySQLDbConnection>()
    .AddDefaultTokenProviders();

builder.Services.AddSwaggerGen();

builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = false;
    options.Cookie.SameSite = SameSiteMode.None;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);

    options.LoginPath = "/api/Login";
    options.AccessDeniedPath = "/ilyennincs";
    options.SlidingExpiration = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseCors(x => x
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed(origin => true) // allow any origin
                    .AllowCredentials()); // allow credentials

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI(options => {
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = "swagger";
});

app.Run();

