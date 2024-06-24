using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[Authorize]
public class ConsoleController : Controller
{
    private static readonly string logFileName = $"log{DateTime.Now:yyyyMMdd}.txt";
    private static readonly string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", logFileName);

    public IActionResult Index()
    {
        string logContent = "";

        if (System.IO.File.Exists(logFilePath))
        {
            using var fileStream = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
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
