using Clone.Models;
using Clone.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Clone.Web.Components.Pages.UserCredentials
{
    public partial class UserCredentials
    {
        private const string MESSAGEBAR_SECTION = "MESSAGEBAR_USERCREDENTIAL";
        private FluentDataGrid<UserCredential>? _Grid;

        private UserCredential _UserCredential { get; set; } = new();
        private UserCredential _UserCredentialsDetails { get; set; } = new();
        private IQueryable<UserCredential>? _UserCredentials { get; set; }

        private IEnumerable<ApplicationUser> _ApplicationUserSelect = default!;
        private List<string> _StatusSelect = default!;

        #region Search
        private string _HashTokenSearch = string.Empty;
        private bool _SearchButtonLoading = false;
        #endregion

        #region Filter
        private bool _CollapsedContent = true;
        private bool _CollapsedDetails = true;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            await LoadSelects();
            await SearchData();
        }
        private async Task LoadSelects()
        {
            _ApplicationUserSelect = await DbContext.Users
                .AsNoTracking()
                .OrderBy(u => u.Name)
                .ToListAsync();

            _StatusSelect = Enum.GetValues<CredentialStatus>()
                .Select(e => e.GetDisplayName())
                .ToList();
        }

        private async Task KeyDownHandler(FluentKeyCodeEventArgs e)
        {
            if (e.KeyCode == Constants.KeyCodeEnter)
            {
                await SearchData();
            }
        }

        private void OnAddClicked()
        {
            _UserCredential = new UserCredential
            {
                Expired = DateTime.UtcNow.AddDays(7),
                Status = CredentialStatus.pending.GetDisplayName()
            };
            _CollapsedContent = false;
        }

        private void OnCancelContentClicked()
        {
            _CollapsedContent = true;
        }

        private void OnCancelDetailsClicked()
        {
            _CollapsedDetails = true;
        }

        private async Task OnCopyDetailsToClipboard()
        {
            if (_UserCredentialsDetails is null)
            {
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: "No UserCredential selected.");
                return;
            }

            try
            {
                var json = JsonSerializer.Serialize(_UserCredentialsDetails, new JsonSerializerOptions { WriteIndented = true });
                await JS.InvokeVoidAsync("navigator.clipboard.writeText", json);

                await NotificationService.ShowMessageBarAsync(options =>
                {
                    options.Section = MESSAGEBAR_SECTION;
                    options.Intent = MessageBarIntent.Info;
                    options.Layout = MessageBarLayout.SingleLine;
                    options.Title = $"UserCredential Id: {_UserCredentialsDetails.Id} copied to clipboard!";
                    options.AllowDismiss = true;
                    options.Lifetime = TimeSpan.FromSeconds(3);
                    options.ResultTiming = MessageBarResultTiming.Closed;
                });
            }
            catch (JSException ex)
            {
                Logger.LogError(ex, "Error copying UserCredential {Id} to clipboard.", _UserCredentialsDetails.Id);
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: "Could not copy the content to the clipboard.");
            }
        }

        private async Task OnDeleteClicked(int id)
        {
            var repository = UnitOfWork.Repository<UserCredential>();
            var toDelete = await repository.GetByIdAsync(id);

            if (toDelete is null)
            {
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: $"UserCredential Id: {id} not found.");
                return;
            }

            var result = await DialogService.ShowConfirmationAsync($"Are you sure you want to <strong>delete</strong> item Id: {toDelete.Id}?");
            if (result.Cancelled)
            {
                return;
            }

            try
            {
                await repository.RemoveAsync(toDelete);
                await UnitOfWork.SaveAsync();
                await SearchData();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Logger.LogError(ex, "Concurrency error while deleting UserCredential {Id}.", id);
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: "The item was modified by another user. Refresh and try again.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error while deleting UserCredential {Id}.", id);
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: "An error occurred while deleting the data.");
            }
        }

        private async Task OnDetailsClicked(int id)
        {
            var toDetails = await DbContext.UserCredentials
                .Include(u => u.ApplicationUser)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);

            if (toDetails is null)
            {
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: $"UserCredential Id: {id} not found.");
                return;
            }

            _UserCredentialsDetails = toDetails;
            _CollapsedDetails = false;
        }

        private async Task OnEditClicked(int id)
        {
            var repository = UnitOfWork.Repository<UserCredential>();
            var toEdit = await repository.GetByIdAsync(id);

            if (toEdit is null)
            {
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: $"UserCredential Id: {id} not found.");
                return;
            }

            _UserCredential = toEdit;
            _CollapsedContent = false;
        }

        private async Task OnSaveContentClicked()
        {
            if (string.IsNullOrWhiteSpace(_UserCredential.HashToken))
            {
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: "HashToken is required.");
                return;
            }

            if (string.IsNullOrWhiteSpace(_UserCredential.ApplicationUserId))
            {
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: "ApplicationUser is required.");
                return;
            }

            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var repository = UnitOfWork.Repository<UserCredential>();

            try
            {
                if (_UserCredential.Id == 0)
                {
                    _UserCredential.InsertAudit(authState.User);
                    await repository.AddAsync(_UserCredential);
                }
                else
                {
                    _UserCredential.UpdateAudit(authState.User);
                    await repository.UpdateAsync(_UserCredential);
                }

                await UnitOfWork.SaveAsync();

                await SearchData();
                ResetForm();
                _CollapsedContent = true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Logger.LogError(ex, "Concurrency error UserCredential Id: {Id}.", _UserCredential.Id);
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: "Concurrency error. Cancel the operation and try again.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error UserCredential Id: {Id}.", _UserCredential.Id);
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: "An error occurred while saving the data.");
            }
        }

        private async Task OnSearchClicked()
        {
            await SearchData();
        }

        private void ResetForm()
        {
            _UserCredential = new UserCredential();
        }

        private async Task SearchData()
        {
            _SearchButtonLoading = true;

            try
            {
                var findeds = await DbContext.UserCredentials
                    .Include(u => u.ApplicationUser)
                    .Where(d => string.IsNullOrWhiteSpace(_HashTokenSearch) || d.HashToken.Contains(_HashTokenSearch))
                    .AsNoTracking()
                    .OrderByDescending(d => d.Id)
                    .Take(MaxRows.MaxThirty)
                    .ToListAsync();

                if (findeds.Count >= MaxRows.MaxThirty)
                {
                    await NotificationService.ShowWarningBarAsync(MESSAGEBAR_SECTION, title: $"Your search returned more than {MaxRows.MaxThirty} results. Improve your filter.");
                }

                _UserCredentials = findeds.AsQueryable();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error searching UserCredentials.");
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: "An error occurred while searching the data.");
            }
            finally
            {
                _SearchButtonLoading = false;
                await InvokeAsync(StateHasChanged);
            }
        }
    }

    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            var memberInfo = enumValue.GetType().GetMember(enumValue.ToString());
            if (memberInfo.Length > 0)
            {
                var attrs = memberInfo[0].GetCustomAttributes(typeof(DisplayAttribute), false);
                if (attrs.Length > 0)
                    return ((DisplayAttribute)attrs[0]).Name;
            }
            return enumValue.ToString();
        }
    }
}
