using System;
using System.Collections.Generic;

namespace acl_openstack_identity.Models;

public partial class User
{
    public long Id { get; set; }

    public string? Email { get; set; }

    public string? Name { get; set; }

    public string? Lastname { get; set; }

    public string? Phone { get; set; }

    public string? Password { get; set; }

    public string? Icon { get; set; }

    public bool? Isfirstlogin { get; set; }

    public string? Openstackpassword { get; set; }

    public string? OpenstackId { get; set; }

    public virtual ICollection<ProjectsUser> ProjectsUsers { get; set; } = new List<ProjectsUser>();

    public virtual ICollection<UsersRole> UsersRoles { get; set; } = new List<UsersRole>();

    public virtual ICollection<UsersToken> UsersTokens { get; set; } = new List<UsersToken>();
}
