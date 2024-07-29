using System;
using System.Collections.Generic;

namespace acl_openstack_identity.Models;

public partial class UsersRole
{
    public long Id { get; set; }

    public long RoleId { get; set; }

    public long UserId { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
