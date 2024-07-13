using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Server.Repository.Interfaces;
using Server.Services.Interfaces;

namespace Server.Controllers;

[Authorize]
public class ClientsController(
    ICommandExecutor commandExecutor,
    IConnectionService connectionService,
    IClientRepository clientRepository
    ) : Controller
{
    private readonly ICommandExecutor _commandExecutor = commandExecutor;
    private readonly IConnectionService _connectionService = connectionService;
    private readonly IClientRepository _clientRepository = clientRepository;

    public async Task<IActionResult> Index()
    {
        var connectionIds = _connectionService.GetIdsList();
        var clientConnections = await _clientRepository.GetClientConnectionsAsync(connectionIds);

        return View(clientConnections);
    }

    public IActionResult Individual(string connectionGuid)
    {
        if (!_connectionService.ConnectionExists(connectionGuid))
        {
            return NotFound();
        }

        Message command = new() { ConnectionGuid = connectionGuid };

        return View(command);
    }

    [HttpPost]
    public async Task<IActionResult> ExecuteCommand([FromBody] Message commandModel)
    {
        await _commandExecutor.ProcessCommand(commandModel);

        return Json(new { success = true });
    }

    public async Task<IActionResult> Disconnect(string connectionGuid)
    {
        if (!_connectionService.ConnectionExists(connectionGuid))
        {
            return NotFound();
        }

        await _connectionService.CloseAsync(connectionGuid);
        await Task.Delay(50);

        return RedirectToAction("Index");
    }
}
