using System;
using System.Collections.Generic;

namespace AcademyGraduationSupply.Models;

public partial class Supplier
{
    public int SupplierId { get; set; }

    public string SupplierName { get; set; } = null!;

    public int CategoryId { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Offer? Offer { get; set; }
}
