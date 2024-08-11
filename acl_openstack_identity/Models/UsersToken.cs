using System;
using System.Collections.Generic;

namespace acl_openstack_identity.Models;

public partial class UsersToken
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public string? Token { get; set; }

    public DateTime? Expire { get; set; }

    public virtual User User { get; set; } = null!;
}
