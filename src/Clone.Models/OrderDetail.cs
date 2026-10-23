using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clone.Models
{
    public class OrderDetail : AuditableEntity
    {
        [MaxLength(50)]
        public string ProductRetailerId { get; set; } = string.Empty;
        
        public int Quantity { get; set; }
        
        public decimal ItemPrice { get; set; }
        
        [MaxLength(50)]
        public string Currency { get; set; } = string.Empty;

        [ForeignKey("OrderHeaderId")]
        [Required(ErrorMessage = "The {0} field is required.")]
        public int OrderHeaderId { get; set; }
        public virtual OrderHeader OrderHeader { get; set; } = default!;
    }
}
