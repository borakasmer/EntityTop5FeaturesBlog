using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class Actions
{
    public int Id { get; set; }

    public long? ActionNumber { get; set; }

    public string? ActionName { get; set; }

    public int? IdModule { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual Modules? IdModuleNavigation { get; set; }
}
