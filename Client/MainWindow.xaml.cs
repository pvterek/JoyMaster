using Client.Services;
using System.Windows;

namespace Client;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        InitializeAsync();
    }

    private async void InitializeAsync()
    {
        await RunServices();
    }

    public async Task RunServices()
    {
        var client = new ClientService();
        await client.Run();
    }
}