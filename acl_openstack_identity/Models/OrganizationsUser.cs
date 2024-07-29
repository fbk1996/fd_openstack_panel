using System;
using System.Collections.Generic;

namespace acl_openstack_identity.Models;

public partial class OrganizationsUser
{
    public long Id { get; set; }

    public long OrganizationId { get; set; }

    public long UserId { get; set; }

    public virtual Organisation Organization { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
