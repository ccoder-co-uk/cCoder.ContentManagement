// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Rendering.Models;

internal sealed class PageRenderUser
{
    public string Id { get; set; }
    public string DefaultCultureId { get; set; }
    public string DisplayName { get; set; }
    public string Email { get; set; }
    public IReadOnlyDictionary<int, ISet<string>> AppPrivileges { get; set; }
    public bool Can(int? appId, string operation)
    {
        string normalizedOperation = operation?.ToLowerInvariant() ?? string.Empty;

        if (!appId.HasValue)
        {
            return AppPrivileges.Values.Any(predicate: privileges => privileges.Contains(item: normalizedOperation));
        }

        return AppPrivileges.TryGetValue(key: appId.Value, value: out ISet<string> value)
            && value.Contains(item: normalizedOperation);
    }

    internal PageRenderUser
    ()
    {
        this.Id = string.Empty;
        this.DefaultCultureId = string.Empty;
        this.DisplayName = string.Empty;
        this.Email = string.Empty;
        this.AppPrivileges = new Dictionary<int, ISet<string>>();
    }
}