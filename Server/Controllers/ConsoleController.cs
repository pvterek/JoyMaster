using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

public class ConsoleController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
