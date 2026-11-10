using System;
using System.Collections.Generic;

namespace TASK7.Models;

public partial class Order
{
    public int Id { get; set; }

    public string CustomerName { get; set; } = null!;

    public DateTime OrderDate { get; set; }

    public decimal TotalPrice { get; set; }

    public string Status { get; set; } = null!;

    public int EmployeeId { get; set; }

    public virtual Employee Employee { get; set; } = null!;
}
