// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal interface IAppService
{
    App GetApp(int appId, bool ignoreFilters = false);

    ValueTask<App> GetAppForRenderAsync(int appId);

    App GetAppForDelete(int appId);

    IQueryable<App> GetAllApp(bool ignoreFilters = false);

    ValueTask<App> AddAppAsync(App newApp);

    ValueTask<App> UpdateAppAsync(App updatedApp);

    ValueTask DeleteAsync(int appId);

    string GetRequestPath();

    string GetRequestHost();

    IQueryable<Culture> GetAllCultures();

    IQueryable<Privilege> GetAllPrivileges();

    User GetCurrentUser();

    string GetCurrentUserId();

    bool IsAdminOfApp(int appId);

    void Authorize(int? appId, string privilege);

    ValueTask<Role> AddRoleAsync(Role newRole);

    ValueTask<Role> UpdateRoleAsync(Role updatedRole);

    IQueryable<Role> GetAllRolesIgnoringFilters();

    ValueTask<UserRole> AddUserRoleAsync(UserRole newUserRole);

    IQueryable<UserRole> GetAllUserRolesIgnoringFilters();

    ValueTask DeleteAllUserRolesAsync(IEnumerable<UserRole> deletedUserRole);

    IQueryable<Page> GetAllPagesIgnoringFilters();

    ValueTask<Page> UpdatePageAsync(Page updatedPage);
}