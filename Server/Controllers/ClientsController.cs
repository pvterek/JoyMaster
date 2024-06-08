using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Server.Services;

namespace Server.Controllers;

public class ClientsController(ManageClientService manageClientService) : Controller
{
    private readonly ManageClientService _manageClientService = manageClientService;

    public IActionResult Index()
    {
        var clients = HandlerService.ConnectedClients.Keys.ToList();

        return View(clients);
    }

    public IActionResult Individual(string id)
    {
        var client = HandlerService.ConnectedClients.Keys.FirstOrDefault(c => c.Id == id);
        if (client == null)
        {
            return NotFound();
        }

        CommandModel command = new() { ClientId = id };

        return View(command);
    }

    [HttpPost]
    public async Task<IActionResult> ExecuteCommand([FromBody] CommandModel commandModel)
    {
        await _manageClientService.ProcessCommand(commandModel);

        return Json(new { success = true });
    }

    public async Task<IActionResult> Disconnect(string id)
    {
        var client = HandlerService.ConnectedClients.Keys.FirstOrDefault(c => c.Id == id);
        if (client == null)
        {
            return NotFound();
        }

        CommandModel endCommand = new()
        {
            ClientId = id,
            Command = "end"
        };
        await _manageClientService.ProcessCommand(endCommand);
        await Task.Delay(100);

        return RedirectToAction("Index");
    }
}
