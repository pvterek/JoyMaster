using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Services;
using Server.Services.Interfaces;
using Server.Utilities;
using Server.Utilities.Exceptions;
using Server.Utilities.Hubs;
using Server.Utilities.Logs;

namespace Server;

public class Program
{
    private static FileStream logFileStream = null!;
    private static StreamWriter logFileWriter = null!;

    public static string logFilePath = @$"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\logi\console.log";

    public static void Main(string[] args)
    {
        logFileStream = new FileStream(logFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
        logFileWriter = new StreamWriter(logFileStream) { AutoFlush = true };

        Console.SetOut(new AnsiStrippingTextWriter(logFileWriter));
        Console.SetError(new AnsiStrippingTextWriter(logFileWriter));

        try
        {
            var builder = WebApplication.CreateBuilder(args);
            Configure(builder);

            var app = builder.Build();
            Setup(app);
            app.Run();
        }
        finally
        {
            logFileWriter.Close();
            logFileStream.Close();
        }
    }

    private static void Configure(WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<ApplicationDbContext>();
        builder.Services.AddControllersWithViews();

        builder.Services.AddGrpc(options =>
        {
            options.Interceptors.Add<ExceptionInterceptor>();
            options.MaxReceiveMessageSize = 262144;
            options.MaxSendMessageSize = 262144;
        });

        builder.Services.AddSingleton<IClientDictionary, ClientDictionary>();

        builder.Services.AddScoped<IMessageSender, MessageSender>();
        builder.Services.AddScoped<LoggerHelper>();
        builder.Services.AddScoped<LoggerService>();
        builder.Services.AddScoped<HandlerService>();
        builder.Services.AddScoped<ManageClientService>();
        builder.Services.AddScoped<IClientService, ClientService>();

        builder.Services.AddSignalR();
    }

    private static void Setup(WebApplication app)
    {
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
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Dashboard}/{action=Index}/{id?}");
        app.MapRazorPages();
        app.MapGrpcService<HandlerService>();

        app.MapHub<ConsoleHub>("/consoleHub");
    }
}