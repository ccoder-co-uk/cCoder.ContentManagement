using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Brokers;

public interface IAuthorizationBroker
{
    User GetCurrentUser();

    bool IsAdminOfApp(int? appId);

    bool IsAdmin(int appId, string userName);

    void Authorize(int? appId, string privilege);
}
