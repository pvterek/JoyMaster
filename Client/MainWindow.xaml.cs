using Client.Services;
using System.Windows;

namespace Client;

public partial class MainWindow : Window
{
    private ConnectionHandler _connectionHandler = null!;

    public MainWindow()
    {
        InitializeComponent();
    }

    public void Initialize(ConnectionHandler connectionHandler)
    {
        _connectionHandler = connectionHandler;
        InitializeAsync();
    }

    private async void InitializeAsync()
    {
        await RunServices();
    }

    public async Task RunServices()
    {
        await _connectionHandler.HandleConnectionAsync();
    }
}