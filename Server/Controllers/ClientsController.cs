using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Server.Repository;
using Server.Services;
using Server.Services.Interfaces;
using Server.Utilities.Constants;

namespace Server.Controllers;

[Authorize]
public class ClientsController(
    ManageClientService manageClientService,
    IActiveConnections activeConnections,
    IClientRepository clientRepository
    ) : Controller
{
    private readonly ManageClientService _manageClientService = manageClientService;
    private readonly IActiveConnections _activeConnections = activeConnections;
    private readonly IClientRepository _clientRepository = clientRepository;

    public async Task<IActionResult> Index()
    {
        var connectionIds = _activeConnections.Connections.Keys.Select(c => c.Id).ToList();
        var clientConnections = await _clientRepository.GetClientConnectionsAsync(connectionIds);

        return View(clientConnections);
    }

    public IActionResult Individual(string connectionGuid)
    {
        var connection = _activeConnections.Connections.Keys.FirstOrDefault(c => c.ConnectionGuid == connectionGuid);
        if (connection == null)
        {
            return NotFound();
        }

        Message command = new() { ConnectionGuid = connectionGuid };

        return View(command);
    }

    [HttpPost]
    public async Task<IActionResult> ExecuteCommand([FromBody] Message commandModel)
    {
        await _manageClientService.ProcessCommand(commandModel);

        return Json(new { success = true });
    }

    public async Task<IActionResult> Disconnect(string connectionGuid)
    {
        var connection = _activeConnections.Connections.Keys.FirstOrDefault(c => c.ConnectionGuid == connectionGuid);
        if (connection == null)
        {
            return NotFound();
        }

        Message endCommand = new()
        {
            ConnectionGuid = connectionGuid,
            MessageContent = AppConstants.EndCommand
        };
        await _manageClientService.ProcessCommand(endCommand);
        await Task.Delay(100);

        return RedirectToAction("Index");
    }
}
