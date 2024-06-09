using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

public class ConsoleController : Controller
{
    public IActionResult Index()
    {
        string logContent = "";

        if (System.IO.File.Exists(Program.logFilePath))
        {
            using var fileStream = new FileStream(Program.logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var streamReader = new StreamReader(fileStream);
            logContent = streamReader.ReadToEnd();
        }
        else
        {
            logContent = "Log file not found.";
        }

        return View((object)logContent);
    }
}
