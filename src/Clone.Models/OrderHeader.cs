using System.ComponentModel.DataAnnotations;

namespace Clone.Models
{
    public sealed class OrderHeader : AuditableEntity
    {
        [MaxLength(50)]
        public string DisplayPhoneNumber { get; set; } = string.Empty;

        [MaxLength(50)]
        public string PhoneNumberId { get; set; } = string.Empty;

        [MaxLength(50)]
        public string ProfileName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string WaId { get; set; } = string.Empty;

        [MaxLength(50)]
        public string From { get; set; } = string.Empty;

        [MaxLength(120)]
        public string FromId { get; set; } = string.Empty;

        [MaxLength(50)]
        public string CatalogId { get; set; } = string.Empty;

        [MaxLength(120)]
        public string Text { get; set; } = string.Empty;

        public decimal Total { get; set; } = 0;
    }
}
