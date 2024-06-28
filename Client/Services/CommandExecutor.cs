using System.Diagnostics;
using System.IO;
using System.Text;

namespace Client.Services;

public class CommandExecutor
{
    public async Task<string> ExecuteCommand(string command)
    {
        try
        {
            var startInfo = CreateProcessStartInfo(command);

            using var process = StartProcess(startInfo);

            var (StandardOutput, StandardError) = await CaptureProcessOutputAsync(process);

            return string.IsNullOrEmpty(StandardOutput) ? StandardError : StandardOutput;
        }
        catch (Exception ex)
        {
            return $"Exception: {ex.Message}";
        }
    }

    private ProcessStartInfo CreateProcessStartInfo(string command)
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

    private Process StartProcess(ProcessStartInfo startInfo)
    {
        var process = new Process { StartInfo = startInfo };
        process.Start();
        return process;
    }

    private async Task<(string StandardOutput, string StandardError)> CaptureProcessOutputAsync(Process process)
    {
        var outputTask = ReadStreamAsync(process.StandardOutput);
        var errorTask = ReadStreamAsync(process.StandardError);

        await Task.WhenAll(outputTask, errorTask);

        process.WaitForExit();

        return (await outputTask, await errorTask);
    }

    private async Task<string> ReadStreamAsync(StreamReader reader)
    {
        var output = new StringBuilder();
        while (!reader.EndOfStream)
        {
            output.AppendLine(await reader.ReadLineAsync());
        }

        return output.ToString().Trim();
    }
}
