using Messzendzser.Controllers;
using Messzendzser.Model.DB;
using Messzendzser.Voip;
using Messzendzser.WhiteBoard;
using System.Net;


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

// Add DbContext service
builder.Services.AddDbContext<IDataSource,MySQLDbConnection>();

builder.Services.AddSwaggerGen();

WhiteboardManager.JsonTest();

WhiteboardManager.JsonMessageEventTest();

builder.Services.AddScoped<WhiteboardManager>();

builder.Services.AddScoped<MessageSenderHub>();

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

// Websockets

var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};

app.UseWebSockets(webSocketOptions);

// Websockets

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI(options => {
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = "swagger";
});

app.Run();

