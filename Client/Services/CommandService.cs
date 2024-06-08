using System.Diagnostics;
using System.Text;

namespace Client.Services;

public class CommandService()
{
    public static async Task<string> ExecuteCommand(string command)
    {
        try
        {
            var startInfo = CreateProcessStartInfo(command);

            using var process = new Process { StartInfo = startInfo };

            process.Start();

            var outputTask = ReadStreamAsync(process.StandardOutput);
            var errorTask = ReadStreamAsync(process.StandardError);

            await Task.WhenAll(outputTask, errorTask);

            string output = outputTask.Result;
            string error = errorTask.Result;

            process.WaitForExit();

            return string.IsNullOrEmpty(output) ? error : output;
        }
        catch (Exception ex)
        {
            return $"Exception: {ex.Message}";
        }
    }

    private static ProcessStartInfo CreateProcessStartInfo(string command)
    {
        return new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/c {command}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
    }

    private static async Task<string> ReadStreamAsync(StreamReader reader)
    {
        var output = new StringBuilder();
        while (!reader.EndOfStream)
        {
            output.AppendLine(await reader.ReadLineAsync());
        }

        return output.ToString().Trim();
    }
}
