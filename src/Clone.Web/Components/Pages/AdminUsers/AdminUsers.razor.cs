using Clone.Models;
using Clone.Models.Dtos;
using Clone.Utility;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;
using Serilog.Core;

namespace Clone.Web.Components.Pages.AdminUsers
{
    public partial class AdminUsers
    {
        const string MESSAGEBAR_SECTION = "MESSAGEBAR_SERVICE_DEFAULT";
        FluentDataGrid<UserBySearchDto>? _Grid;

        [Inject]
        public IJSRuntime JS { get; set; } = default!;
        ApplicationUser _ApplicationUser { get; set; } = default!;
        UserBySearchDto _UserBySearchDtoDetails { get; set; } = default!;
        IQueryable<UserBySearchDto>? _UserBySearchDto { get; set; }

        #region Search
        string _NameSearch = string.Empty;
        string _PhoneNumberSearch = string.Empty;
        bool _SearchButtonLoading = false;
        #endregion

        #region Filter
        private bool _CollapsedContent = true;
        private bool _CollapsedDetails = true;
        #endregion

        #region Select
        IEnumerable<SelectListItem> _RolesSelect = Lists.RoleList;
        string _RoleSelected = RD.RoleCustomer;
        IEnumerable<Company> _CompaniesSelect = default!;
        bool _CompaniesSelectHidden = true;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            await SearchData();
        }

        private async Task KeyDownHandler(FluentKeyCodeEventArgs e)
        {
            if (e.KeyCode == Clone.Utility.Constants.KeyCodeEnter)
            {
                await SearchData();
            }
        }

        private void OnCancelContentClicked()
        {
            _CollapsedContent = true;
        }

        private void OnCancelDetailsClicked()
        {
            _CollapsedDetails = true;
        }

        private async Task OnCopyEmailToClipboard()
        {
            if (_UserBySearchDtoDetails is null)
            {
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: Localizer["Error.UserNotSelected"]);
            }
            await JS.InvokeVoidAsync("navigator.clipboard.writeText", _UserBySearchDtoDetails!.Email);
            await NotificationService.ShowMessageBarAsync(options =>
            {
                options.Section = MESSAGEBAR_SECTION;

                options.Intent = MessageBarIntent.Info;
                options.Layout = MessageBarLayout.SingleLine;
                options.Title = Localizer["Message.EmailCopied", _UserBySearchDtoDetails.Email ?? "NotEmail"];
                options.AllowDismiss = true;
                options.Lifetime = TimeSpan.FromSeconds(3);
                options.ResultTiming = MessageBarResultTiming.Closed;
            });
        }

        private async Task OnLockUnlockClicked(UserBySearchDto context)
        {
            var action = context.IsLocked ? SharedLocalizer["Common.Unlock"] : SharedLocalizer["Common.Lock"];
            var confirm = Localizer["Confirm.LockUnlockUser", action];
            var result = await DialogService.ShowConfirmationAsync(confirm);
            if (result.Cancelled) return;

            var user = await _UserManager.FindByIdAsync(context.Id);
            if (user is null)
            {
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: SharedLocalizer["Error.UserNotFound", context.Id]); 
                return;
            }

            var now = DateTimeOffset.UtcNow;
            DateTimeOffset? newLockout = (user.LockoutEnd != null && user.LockoutEnd > now)
                ? now
                : now.AddYears(1000);

            var Lockout = await _UserManager.SetLockoutEndDateAsync(user, newLockout);

            if (!Lockout.Succeeded)
            {
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: Localizer["Error.LockUnlockUser", action]);
                return;
            }

            await SearchData();
        }

        private async Task OnDetailsClicked(string id)
        {
            var toDetails = await _UnitOfWork.UserBySearch(id, "", "").ToListAsync();
            if (toDetails is null)
            {
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: SharedLocalizer["Error.UserNotFound", id]);
                return;
            }

            _UserBySearchDtoDetails = toDetails!.First();
            _CollapsedDetails = false;
        }

        private async Task OnEditClicked(string id)
        {
            _CompaniesSelectHidden = true;

            var toEdit = await _UserManager.FindByIdAsync(id);
            if (toEdit is null)
            {
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: SharedLocalizer["Error.UserNotFound", id]);
                return;
            }
            _CompaniesSelect = await _UnitOfWork.Repository<Company>().GetAllAsync();
            _ApplicationUser = toEdit;

            var userRole = await _UserManager.GetRolesAsync(_ApplicationUser);
            string? rol = userRole.FirstOrDefault();
            _RoleSelected = rol ?? RD.RoleCustomer;

            if (_ApplicationUser.CompanyId != null)
            {
                _CompaniesSelectHidden = false;
            }

            _CollapsedContent = false;
        }

        private async Task OnRoleChangedChangeEvent(ChangeEventArgs e)
        {
            _RoleSelected = e.Value?.ToString() ?? string.Empty;

            if(_RoleSelected == RD.RoleSupplier)
            {
                _CompaniesSelectHidden = false;
            }
            else
            {
                _CompaniesSelectHidden = true;
                _ApplicationUser.CompanyId = null;
            }

            await Task.CompletedTask;
        }

        private async Task OnSaveContentClicked()
        {
            var toChangeRol = await _UserManager.FindByIdAsync(_ApplicationUser.Id);
            if (toChangeRol is null)
            {
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: SharedLocalizer["Error.UserNotFound", _ApplicationUser.Id]);
                return;
            }

            var userRole = await _UserManager.GetRolesAsync(toChangeRol);
            string? oldRole = userRole.FirstOrDefault();

            if (oldRole is null)
            {
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: SharedLocalizer["Error.UserNotRol", _ApplicationUser.Id]);
                return;
            }

            if (oldRole != _RoleSelected)
            {
                var removeRol = await _UserManager.RemoveFromRoleAsync(toChangeRol, oldRole);
                if(!removeRol.Succeeded)
                {
                    await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: SharedLocalizer["Error.RemoveRol"]);
                    return;
                }
                var addRol = await _UserManager.AddToRoleAsync(toChangeRol, _RoleSelected);
                if(!addRol.Succeeded)
                {
                    await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: SharedLocalizer["Error.AddRol"]);
                    return;
                }
            }

            if (oldRole != _RoleSelected && oldRole == RD.RoleSupplier)
            {
                toChangeRol.CompanyId = null;
            }

            await _UnitOfWork.SaveAsync();


            await SearchData();
            _CollapsedContent = true;
        }

        private async void OnSearchClicked()
        {
            await SearchData();
        }

        private async Task SearchData()
        {
            _SearchButtonLoading = true;
            try
            {
                var findeds = await (_UnitOfWork.UserBySearch("", _NameSearch, _PhoneNumberSearch)).ToListAsync();

                if (findeds.Count >= MaxRows.MaxThirty)
                {
                    await NotificationService.ShowWarningBarAsync(MESSAGEBAR_SECTION, title: SharedLocalizer["Warning.Search", MaxRows.MaxThirty]);
                }

                _UserBySearchDto = findeds.AsQueryable();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error searching Companies.");
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: "An error occurred while searching the data.");
            }
            finally
            {
                _SearchButtonLoading = false;
                await InvokeAsync(StateHasChanged);
            }
        }
    }
}
