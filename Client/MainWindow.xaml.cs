using Client.ConnectionHandlers;
using System.Windows;

namespace Client;

public partial class MainWindow : Window
{
    private CommandStreamHandler _connectionHandler = null!;

    public MainWindow()
    {
        InitializeComponent();
    }

    public void Initialize(CommandStreamHandler connectionHandler)
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