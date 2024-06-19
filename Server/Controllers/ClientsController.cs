using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Server.Services;
using Server.Services.Interfaces;
using Server.Utilities.Constants;

namespace Server.Controllers;

[Authorize]
public class ClientsController(
    ManageClientService manageClientService,
    IClientDictionary clientDictionary
    ) : Controller
{
    private readonly ManageClientService _manageClientService = manageClientService;
    private readonly IClientDictionary _clientDictionary = clientDictionary;

    public IActionResult Index()
    {
        var clients = _clientDictionary.Clients.Keys.ToList();

        return View(clients);
    }

    public IActionResult Individual(string id)
    {
        var client = _clientDictionary.Clients.Keys.FirstOrDefault(c => c.Id == id);
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
        var client = _clientDictionary.Clients.Keys.FirstOrDefault(c => c.Id == id);
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
