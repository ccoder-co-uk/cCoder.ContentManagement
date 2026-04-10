namespace cCoder.ContentManagement.Models;

public class Replacement
{
    private readonly string newString;

    public string Old { get; }

    public string New => newString ?? ReplaceFunction(Old);

    public Func<string, string> ReplaceFunction { get; } = (string source) => source;

    public Replacement(string old, string @new)
    {
        Old = old;
        newString = @new;
    }

    public Replacement(string old, Func<string, string> replacer)
    {
        Old = old;
        if (replacer != null)
        {
            ReplaceFunction = replacer;
        }
    }
}
