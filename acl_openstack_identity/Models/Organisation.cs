using System;
using System.Collections.Generic;

namespace acl_openstack_identity.Models;

public partial class Organisation
{
    public long Id { get; set; }

    public string? Name { get; set; }

    public string OpenstackProjectId { get; set; } = null!;

    public string? GroupId { get; set; }

    public virtual ICollection<OrganizationsUser> OrganizationsUsers { get; set; } = new List<OrganizationsUser>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
