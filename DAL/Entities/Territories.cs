using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class Territories
{
    public string TerritoryId { get; set; } = null!;

    public string TerritoryDescription { get; set; } = null!;

    public int RegionId { get; set; }

    public virtual Region Region { get; set; } = null!;

    public virtual ICollection<Employees> Employee { get; set; } = new List<Employees>();
}
