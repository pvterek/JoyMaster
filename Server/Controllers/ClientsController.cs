using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Server.Services;
using Server.Utilities.Constants;

namespace Server.Controllers;

public class ClientsController(ManageClientService manageClientService, HandlerService handlerService) : Controller
{
    private readonly ManageClientService _manageClientService = manageClientService;
    private readonly HandlerService _handlerService = handlerService;

    public IActionResult Index()
    {
        var clients = _handlerService.connectedClients.Keys.ToList();

        return View(clients);
    }

    public IActionResult Individual(string id)
    {
        var client = _handlerService.connectedClients.Keys.FirstOrDefault(c => c.Id == id);
        if (client == null)
        {
            return NotFound();
        }

        MessageModel command = new() { ClientId = id };

        return View(command);
    }

    [HttpPost]
    public async Task<IActionResult> ExecuteCommand([FromBody] MessageModel commandModel)
    {
        await _manageClientService.ProcessCommand(commandModel);

        return Json(new { success = true });
    }

    public async Task<IActionResult> Disconnect(string id)
    {
        var client = _handlerService.connectedClients.Keys.FirstOrDefault(c => c.Id == id);
        if (client == null)
        {
            return NotFound();
        }

        MessageModel endCommand = new()
        {
            ClientId = id,
            Message = AppConstants.EndCommand
        };
        await _manageClientService.ProcessCommand(endCommand);
        await Task.Delay(100);

        return RedirectToAction("Index");
    }
}
