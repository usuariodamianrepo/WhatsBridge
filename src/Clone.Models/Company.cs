using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Clone.Models
{
    public sealed class Company : AuditableEntity
    {
		[DisplayName("Name")]
		[Required(ErrorMessage = "The {0} field is required.")]
		[MaxLength(120)]
		public string Name { get; set; } = string.Empty;

        [DisplayName("Address")]
		[MaxLength(120)]
		public string? StreetAddress { get; set; }

        [DisplayName("City")]
		[MaxLength(120)]
		public string? City { get; set; }

        [DisplayName("State")]
		[MaxLength(60)]
		public string? State { get; set; }

        [DisplayName("Postal Code")]
		[MaxLength(60)]
		public string? PostalCode { get; set; }

        [DisplayName("Phone")]
		[MaxLength(60)]
		public string? PhoneNumber { get; set; }
    }
}
