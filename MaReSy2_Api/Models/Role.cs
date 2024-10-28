﻿using System;
using System.Collections.Generic;

namespace MaReSy2_Api.Models;

public partial class Role
{
    public int RoleId { get; set; }

    public string Rolename { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
