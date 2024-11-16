using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class Region
{
    public int RegionId { get; set; }

    public string RegionDescription { get; set; } = null!;

    public virtual ICollection<Territories> Territories { get; set; } = new List<Territories>();
}
