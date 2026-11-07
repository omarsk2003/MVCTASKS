namespace TASK5CF.Models;

public class PurchaseOrder
{
    public int Id { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.Now;

    public string Status { get; set; } = "Pending";

    public int SupplierId { get; set; }

    public Supplier Supplier { get; set; } = null!;

    public ICollection<PurchaseOrderItem> PurchaseOrderItems
    { get; set; } = new List<PurchaseOrderItem>();
}