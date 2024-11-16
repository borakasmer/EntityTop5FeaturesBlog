using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class Shippers
{
    public int ShipperId { get; set; }

    public string CompanyName { get; set; } = null!;

    public string? Phone { get; set; }

    public virtual ICollection<Orders> Orders { get; set; } = new List<Orders>();
}
