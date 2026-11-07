using System;
using System.Collections.Generic;

namespace AcademyGraduationSupply.Models;

public partial class SupplyRequest
{
    public int RequestId { get; set; }

    public DateTime? RequestDate { get; set; }

    public virtual ICollection<SelectedOffer> SelectedOffers { get; set; } = new List<SelectedOffer>();
}
