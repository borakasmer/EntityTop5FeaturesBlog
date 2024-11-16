using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class UserAction
{
    public int Id { get; set; }

    public int? IdUser { get; set; }

    public long? TotalAction { get; set; }

    public int? IdModule { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual Modules? IdModuleNavigation { get; set; }

    public virtual Users? IdUserNavigation { get; set; }
}
