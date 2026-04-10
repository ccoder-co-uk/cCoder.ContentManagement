namespace cCoder.ContentManagement.Models;

public sealed class ModelStateError
{
    public string Key { get; set; } = string.Empty;

    public object Value { get; set; }

    public string[] Errors { get; set; } = Array.Empty<string>();
}
