using Clone.Models;
using Clone.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;

namespace Clone.Web.Components.Pages.DocumentTypes
{
    public partial class DocumentTypes
    {
        private const string MESSAGEBAR_SECTION = "MESSAGEBAR_SERVICE_DEFAULT";
        private FluentDataGrid<DocumentType>? _Grid;

        private DocumentType _DocumentType { get; set; } = new();
        private DocumentType _DocumentTypeDetails { get; set; } = new();
        private IQueryable<DocumentType>? _DocumentTypes { get; set; }

        #region Search
        private string _NameSearch = string.Empty;
        private bool _SearchButtonLoading = false;
        #endregion

        #region Filter
        private bool _CollapsedContent = true;
        private bool _CollapsedDetails = true;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            await SearchData();
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
            _DocumentType = new();
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
            if (_DocumentTypeDetails is null)
            {
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: "No document type selected.");
                return;
            }

            try
            {
                var json = JsonSerializer.Serialize(_DocumentTypeDetails, new JsonSerializerOptions { WriteIndented = true });
                await JS.InvokeVoidAsync("navigator.clipboard.writeText", json);

                await NotificationService.ShowMessageBarAsync(options =>
                {
                    options.Section = MESSAGEBAR_SECTION;
                    options.Intent = MessageBarIntent.Info;
                    options.Layout = MessageBarLayout.SingleLine;
                    options.Title = $"Document Type Id: {_DocumentTypeDetails.Id} copied to clipboard!";
                    options.AllowDismiss = true;
                    options.Lifetime = TimeSpan.FromSeconds(3);
                    options.ResultTiming = MessageBarResultTiming.Closed;
                });
            }
            catch (JSException ex)
            {
                Logger.LogError(ex, "Error copying DocumentType {Id} to clipboard.", _DocumentTypeDetails.Id);
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: "Could not copy the content to the clipboard.");
            }
        }

        private async Task OnDeleteClicked(int id)
        {
            var repository = UnitOfWork.Repository<DocumentType>();
            var toDelete = await repository.GetByIdAsync(id);

            if (toDelete is null)
            {
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: $"Document Type Id: {id} not found.");
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
                Logger.LogError(ex, "Concurrency error while deleting DocumentType {Id}.", id);
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: "The item was modified by another user. Refresh and try again.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error while deleting DocumentType {Id}.", id);
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: "An error occurred while deleting the data.");
            }
        }

        private async Task OnDetailsClicked(int id)
        {
            var repository = UnitOfWork.Repository<DocumentType>();
            var toDetails = await repository.GetByIdAsync(id);

            if (toDetails is null)
            {
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: $"Document Type Id: {id} not found.");
                return;
            }

            _DocumentTypeDetails = toDetails;
            _CollapsedDetails = false;
        }

        private async Task OnEditClicked(int id)
        {
            var repository = UnitOfWork.Repository<DocumentType>();
            var toEdit = await repository.GetByIdAsync(id);

            if (toEdit is null)
            {
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: $"Document Type Id: {id} not found.");
                return;
            }

            _DocumentType = toEdit;
            _CollapsedContent = false;
        }

        private async Task OnSaveContentClicked()
        {
            if (string.IsNullOrWhiteSpace(_DocumentType.Name))
            {
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: "Name is required.");
                return;
            }

            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var repository = UnitOfWork.Repository<DocumentType>();

            try
            {
                if (_DocumentType.Id == 0)
                {
                    _DocumentType.InsertAudit(authState.User);
                    await repository.AddAsync(_DocumentType);
                }
                else
                {
                    _DocumentType.UpdateAudit(authState.User);
                    await repository.UpdateAsync(_DocumentType);
                }

                await UnitOfWork.SaveAsync();

                await SearchData();
                ResetForm();
                _CollapsedContent = true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Logger.LogError(ex, "Concurrency error DocumentType Id: {Id}.", _DocumentType.Id);
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: "Concurrency error. Cancel the operation and try again.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error DocumentType Id: {Id}.", _DocumentType.Id);
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: "An error occurred while saving the data.");
            }
        }

        private async Task OnSearchClicked()
        {
            await SearchData();
        }

        private void ResetForm()
        {
            _DocumentType = new DocumentType();
        }

        private async Task SearchData()
        {
            _SearchButtonLoading = true;

            try
            {
                var findeds = await UnitOfWork.Repository<DocumentType>().Entities
                    .Where(d => string.IsNullOrWhiteSpace(_NameSearch) || d.Name.Contains(_NameSearch))
                    .AsNoTracking()
                    .Take(MaxRows.MaxThirty)
                    .ToListAsync();

                if (findeds.Count >= MaxRows.MaxThirty)
                {
                    await NotificationService.ShowWarningBarAsync(MESSAGEBAR_SECTION, title: $"Your search returned more than {MaxRows.MaxThirty} results. Improve your filter.");
                }

                _DocumentTypes = findeds.AsQueryable();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error searching DocumentTypes.");
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