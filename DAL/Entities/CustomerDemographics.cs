using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class CustomerDemographics
{
    public string CustomerTypeId { get; set; } = null!;

    public string? CustomerDesc { get; set; }

    public virtual ICollection<Customers> Customer { get; set; } = new List<Customers>();
}
