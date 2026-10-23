namespace Clone.Utility
{
    using System.ComponentModel.DataAnnotations;

    public enum AlertTypes
    {
        [Display(Name = "General")]
        general = 1,
        [Display(Name = "Pago")]
        payment = 2,
        [Display(Name = "Seguridad")]
        security = 3,
        [Display(Name = "Negocio")]
        business = 4,
    }

    public enum AlertStatus
    {
        [Display(Name = "Pendiente")]
        pending = 1,
        [Display(Name = "Activa")]
        active = 2,
        [Display(Name = "En Progreso")]
        inprogress = 3,
        [Display(Name = "Resuelta")]
        resolved = 4,
        [Display(Name = "Expirada")]
        expired = 5
    }
}
