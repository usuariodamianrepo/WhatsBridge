using Microsoft.AspNetCore.Mvc.Rendering;

namespace Clone.Utility
{
    public static class Lists
    {
        public static IEnumerable<SelectListItem> AlertStatusList => new List<SelectListItem>() {
            new SelectListItem(AlertStatus.pending.GetDisplayName(), AlertStatus.pending.GetDisplayName()),
            new SelectListItem(AlertStatus.active.GetDisplayName(), AlertStatus.active.GetDisplayName()),
            new SelectListItem(AlertStatus.inprogress.GetDisplayName(), AlertStatus.inprogress.GetDisplayName()),
            new SelectListItem(AlertStatus.resolved.GetDisplayName(), AlertStatus.resolved.GetDisplayName()),
        };

        public static IEnumerable<SelectListItem> AlertTypeList => new List<SelectListItem>() {
            new SelectListItem(AlertTypes.general.GetDisplayName(), AlertTypes.general.GetDisplayName()),
            new SelectListItem(AlertTypes.payment.GetDisplayName(), AlertTypes.payment.GetDisplayName()),
            new SelectListItem(AlertTypes.security.GetDisplayName(), AlertTypes.security.GetDisplayName()),
            new SelectListItem(AlertTypes.business.GetDisplayName(), AlertTypes.business.GetDisplayName())
        };

        public static IEnumerable<SelectListItem> RoleList => new List<SelectListItem>() {
            new SelectListItem(RD.RoleCustomer, RD.RoleCustomer),
            new SelectListItem(RD.RoleSupplier, RD.RoleSupplier),
            new SelectListItem(RD.RoleAdmin, RD.RoleAdmin),
            new SelectListItem(RD.RoleEmployee, RD.RoleEmployee)
        };
    }
}