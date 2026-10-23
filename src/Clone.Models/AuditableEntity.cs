using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

namespace Clone.Models
{
    public abstract class AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public uint xmin { get; set; } = 0;

        [Display(Name = "Fecha Ins.")]
        [Column(TypeName = "timestamp with time zone")]
        public DateTime InsertDate { get; set; } = DateTime.UtcNow;
        
        [Display(Name = "Usuario Ins."), MaxLength(128)]
        public string InsertUser { get; set; } = "not logged";
        
        [Display(Name = "Fecha Act.")]
        [Column(TypeName = "timestamp with time zone")]
        public DateTime? UpdateDate { get; set; }
        
        [Display(Name = "Usuario Act."), MaxLength(128)]
        public string? UpdateUser { get; set; }

        public void InsertAudit(ClaimsPrincipal? claims)
        {
            InsertDate = DateTime.UtcNow;
            InsertUser = claims?.Identity?.Name ?? "not logged";
        }

        public void UpdateAudit(ClaimsPrincipal? claims)
        {
            UpdateDate = DateTime.UtcNow;
            UpdateUser = claims?.Identity?.Name ?? "not logged";
        }

        public override string ToString()
        {
            return $"{this.GetType().Name} Id: {Id} ";
        }
    }
}