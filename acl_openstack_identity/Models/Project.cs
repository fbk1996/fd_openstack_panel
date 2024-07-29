using System;
using System.Collections.Generic;

namespace acl_openstack_identity.Models;

public partial class Project
{
    public long Id { get; set; }

    public long ParentId { get; set; }

    public string Name { get; set; } = null!;

    public string OpenstackProjectId { get; set; } = null!;

    public int? DescriptionText { get; set; }

    public DateTime? CreationDate { get; set; }

    public string? Status { get; set; }

    public string? Metadata { get; set; }

    public virtual ICollection<Project> InverseParent { get; set; } = new List<Project>();

    public virtual Project Parent { get; set; } = null!;

    public virtual ICollection<ProjectsUser> ProjectsUsers { get; set; } = new List<ProjectsUser>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
