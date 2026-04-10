namespace cCoder.ContentManagement.Rendering.Models;

internal sealed class PageRenderUser
{
    public string Id { get; set; } = string.Empty;

    public string DefaultCultureId { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public IReadOnlyDictionary<int, ISet<string>> AppPrivileges { get; set; } =
        new Dictionary<int, ISet<string>>();

    public bool Can(int? appId, string operation)
    {
        string normalizedOperation = operation?.ToLowerInvariant() ?? string.Empty;

        if (!appId.HasValue)
        {
            return AppPrivileges.Values.Any(privileges => privileges.Contains(normalizedOperation));
        }

        return AppPrivileges.TryGetValue(appId.Value, out ISet<string> value)
            && value.Contains(normalizedOperation);
    }
}
