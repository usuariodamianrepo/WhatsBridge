param(
	[Parameter(Mandatory = $true, HelpMessage = "Provide the Entity Singular Name.")]
    [ValidateNotNullOrEmpty()]
	[string]$EntitySingularName,
	[Parameter(Mandatory = $true, HelpMessage = "Provide the Entity Plural Name.")]
    [ValidateNotNullOrEmpty()]
	[string]$EntityPluralName
)
$entityContent = @"
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clone.Models
{
    public sealed class $EntitySingularName : AuditableEntity
    {
        [Display(Name = "Nombre"), MaxLength(60)]
        public string Name { get; set; } = string.Empty;
        
	    [Display(Name = "Descripción"), MaxLength(450)]
        public string? Description { get; set; }
        
        [Display(Name = "Precio"), Column(TypeName = "decimal(18,2)")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public decimal Price { get; set; }

	    [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Relationship")]
        public int RelationshipId { get; set; }
        [ForeignKey("RelationshipId")]
        [ValidateNever]
        [Display(Name = "Relationship")]
        public Relationship Relationship { get; set; } = default!;
    }
}
"@

New-Item -Path "../src/Clone.Models/$EntitySingularName.cs" -ItemType File -Force
Set-Content -Path "../src/Clone.Models/$EntitySingularName.cs" -Value $entityContent

$entityRepository = -join($EntitySingularName + "Repository")
$entityIRepository = -join("I" + $EntitySingularName + "Repository")
$repContent = @"
using Clone.DataAccess.Repositories.IRepository;
using Clone.Models;

namespace Clone.DataAccess.Repositories
{
    public class $entityRepository : $entityIRepository
    {
        private readonly IRepositoryAsync<$EntitySingularName> _repository;

        public $entityRepository(IRepositoryAsync<$EntitySingularName> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
    }
}
"@

New-Item -Path "../src/Clone.DataAccess/Repositories/$entityRepository.cs" -ItemType File -Force
Set-Content -Path "../src/Clone.DataAccess/Repositories/$entityRepository.cs" -Value $repContent

$repIContent = @"
namespace Clone.DataAccess.Repositories.IRepository
{
	public interface $entityIRepository 
	{

	}
}
"@

New-Item -Path "../src/Clone.DataAccess/Repositories/IRepository/$entityIRepository.cs" -ItemType File -Force
Set-Content -Path "../src/Clone.DataAccess/Repositories/IRepository/$entityIRepository.cs" -Value $repIContent

Add-Content ..\src\Clone.DataAccess\Data\ApplicationDbContext.cs "public DbSet<$EntitySingularName> $EntityPluralName { get; set; }"

Write-Host "Congratulations! the $EntitySingularName entity was created successfully"