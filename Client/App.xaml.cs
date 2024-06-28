using Client.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace Client;

public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;

    public App()
    {
        ServiceCollection services = new();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
    }

    private void ConfigureServices(ServiceCollection services)
    {
        services.AddSingleton<MainWindow>();
        services.AddSingleton<ConnectionService>();

        services.AddScoped<ConnectionHandler>();
        services.AddScoped<CommandExecutor>();
        services.AddScoped<ScreenCaptureService>();
        services.AddScoped<ImageHandler>();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        var connectionHandler = _serviceProvider.GetRequiredService<ConnectionHandler>();

        mainWindow.Initialize(connectionHandler);
    }
}
