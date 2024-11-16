﻿using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class VwCustomerProducts
{
    public string CustomerId { get; set; } = null!;

    public string ProductName { get; set; } = null!;

    public string? ShipCountry { get; set; }

    public decimal UnitPrice { get; set; }

    public short Quantity { get; set; }

    public decimal? Total { get; set; }
}
