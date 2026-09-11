// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Models;

internal sealed class AppOperation
{
    public App App { get; set; }
    public IQueryable<App> Apps { get; set; }
    public IQueryable<Culture> Cultures { get; set; }
    public IQueryable<Privilege> Privileges { get; set; }
    public Role Role { get; set; }
    public IQueryable<Role> Roles { get; set; }
    public UserRole UserRole { get; set; }
    public IQueryable<UserRole> UserRoles { get; set; }
    public UserRole[] DeletedUserRoles { get; set; }
    public Page Page { get; set; }
    public IQueryable<Page> Pages { get; set; }
    public int AppId { get; set; }
    public int? OptionalAppId { get; set; }
    public bool Result { get; set; }
    public string Text { get; set; }
    public string Privilege { get; set; }
}