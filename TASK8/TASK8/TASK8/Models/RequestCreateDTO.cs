using System.ComponentModel.DataAnnotations;
namespace TASK8.Models
{
    public class RequestCreateDTO
    {
        public List<RequestLineDTO> Items { get; set; } = new();
    }

    public class RequestLineDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
