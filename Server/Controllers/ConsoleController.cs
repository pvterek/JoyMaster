using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

public class ConsoleController : Controller
{
    public IActionResult Index()
    {
        string logContent = "";

        string logFileName = $"log{DateTime.Now:yyyyMMdd}.txt";
        string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", logFileName);

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
