using System.Text;
using System.Text.RegularExpressions;

namespace Server.Utilities;

public class AnsiStrippingTextWriter(TextWriter innerWriter) : TextWriter
{
    private readonly TextWriter _innerWriter = innerWriter;

    public override Encoding Encoding => _innerWriter.Encoding;

    public override void Write(char value)
    {
        _innerWriter.Write(value);
    }

    public override void Write(string? value) => _innerWriter.Write(StripAnsiCodes(value));

    public override void WriteLine(string? value) => _innerWriter.WriteLine(StripAnsiCodes(value));

    private static string StripAnsiCodes(string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        var ansiRegex = new Regex(@"\x1B\[[0-9;]*[a-zA-Z]");
        return ansiRegex.Replace(input, string.Empty);
    }
}
