using System;
using System.Collections.Generic;

namespace AcademyGraduationSupply.Models;

public partial class SelectedOffer
{
    public int SelectedOfferId { get; set; }

    public int RequestId { get; set; }

    public int OfferId { get; set; }

    public virtual Offer Offer { get; set; } = null!;

    public virtual SupplyRequest Request { get; set; } = null!;
}
