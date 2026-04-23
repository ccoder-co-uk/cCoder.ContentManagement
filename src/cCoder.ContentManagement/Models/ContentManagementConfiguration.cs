using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Models;

public class ContentManagementConfiguration
{
    public IDictionary<string, string> ConnectionStrings { get; set; } = new Dictionary<string, string>();
    public IDictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();
    public IDictionary<string, string> Services { get; set; } = new Dictionary<string, string>();
    public bool DebugInfo { get; set; }
    public bool LogSQL { get; set; }
    public string RootPath { get; set; } = "Api/ContentManagement";
    public bool IncludeLegacyCoreContext { get; set; } = true;
    public EventProvider[] EventProviders { get; private set; } = [];

    public ContentManagementConfiguration WithEventProviders(params EventProvider[] eventProviders)
    {
        EventProviders = eventProviders ?? [];
        return this;
    }
}
