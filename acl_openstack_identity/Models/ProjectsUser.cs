using System;
using System.Collections.Generic;

namespace acl_openstack_identity.Models;

public partial class ProjectsUser
{
    public long Id { get; set; }

    public long ProjectId { get; set; }

    public long UserId { get; set; }

    public virtual Project Project { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
