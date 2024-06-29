using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Server.ConnectionHandlers;
using Server.ConnectionsHandlers;
using Server.Data;
using Server.Repository;
using Server.Services;
using Server.Services.Interfaces;
using Server.Utilities.Exceptions;
using Server.Utilities.Hubs;
using Server.Utilities.Logs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<AppIdentityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection")));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<AppIdentityDbContext>();

builder.Services.AddRazorPages();

builder.Services.AddControllersWithViews();

builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<ExceptionInterceptor>();
    //options.MaxReceiveMessageSize = 262144;
    //options.MaxSendMessageSize = 262144;
});

builder.Services.AddSingleton<IActiveConnections, ActiveConnections>();

builder.Services.AddScoped<IMessageSender, SenderService>();
builder.Services.AddScoped<IImageSender, SenderService>();
builder.Services.AddScoped<LoggerHelper>();
builder.Services.AddScoped<LoggerService>();
builder.Services.AddScoped<CommandStreamHandler>();
builder.Services.AddScoped<ImageStreamHandler>();
builder.Services.AddScoped<ManageClientService>();
builder.Services.AddScoped<IConnectionService, ConnectionService>();
builder.Services.AddScoped<IConnectionRepository, ConnectionRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<ClientService>();

builder.Services.AddTransient<ImageDataHelper>();

builder.Services.AddSignalR();

builder.Logging.ClearProviders();
var logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs/log.txt"),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 90)
    .WriteTo.Console()
    .CreateLogger();
builder.Logging.AddSerilog(logger);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Dashboard/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.MapRazorPages();

app.MapGrpcService<CommandStreamHandler>();
app.MapGrpcService<ImageStreamHandler>();

app.MapHub<CommandHub>("/commandHub");
app.MapHub<ImageHub>("/imageHub");

app.Run();
