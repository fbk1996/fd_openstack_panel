using System;
using System.Collections.Generic;

namespace acl_openstack_identity.Models;

public partial class Role
{
    public long Id { get; set; }

    public string? Name { get; set; }

    public short? Scope { get; set; }

    public string Permissions { get; set; } = null!;

    public long? ProjectId { get; set; }

    public long OrganisationId { get; set; }

    public virtual Organisation Organisation { get; set; } = null!;

    public virtual Project? Project { get; set; }

    public virtual ICollection<UsersRole> UsersRoles { get; set; } = new List<UsersRole>();
}
