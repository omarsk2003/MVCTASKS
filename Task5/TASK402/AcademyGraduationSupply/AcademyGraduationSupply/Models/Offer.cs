using System;
using System.Collections.Generic;

namespace AcademyGraduationSupply.Models;

public partial class Offer
{
    public int OfferId { get; set; }

    public string OfferName { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public int SupplierId { get; set; }

    public virtual ICollection<SelectedOffer> SelectedOffers { get; set; } = new List<SelectedOffer>();

    public virtual Supplier Supplier { get; set; } = null!;
}
