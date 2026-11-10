using System;
using System.Collections.Generic;

namespace TASK7.Models;

public partial class Employee
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Position { get; set; }

    public decimal? Salary { get; set; }

    public DateOnly? HireDate { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
