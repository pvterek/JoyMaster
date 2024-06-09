using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Exceptions;
using Server.Hubs;
using Server.Services;
using Server.Services.Interfaces;

namespace Server;

public class Program
{
    private static FileStream logFileStream;
    private static StreamWriter logFileWriter;

    public const string logFilePath = @"C:\console.log";

    public static void Main(string[] args)
    {
        logFileStream = new FileStream(logFilePath, FileMode.Create);
        logFileWriter = new StreamWriter(logFileStream) { AutoFlush = true };

        Console.SetOut(logFileWriter);
        Console.SetError(logFileWriter);

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

        builder.Services.AddSingleton<ConsoleService>();
        builder.Services.AddSingleton<HandlerService>();
        builder.Services.AddSingleton<ManageClientService>();
        builder.Services.AddSingleton<IHandlerHelper, HandlerHelper>();

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