using Clone.Models;
using Clone.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;

namespace Clone.Web.Components.Pages.Documents
{
    public partial class Documents
    {
        private const string MESSAGEBAR_SECTION = "MESSAGEBAR_SERVICE_DEFAULT";
        private FluentDataGrid<Document>? _Grid;

        private Document _Document { get; set; } = new();
        private Document _DocumentsDetails { get; set; } = new();
        private IQueryable<Document>? _Documents { get; set; }

        private IEnumerable<DocumentType> _DocumentTypesSelect = default!;

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

        private async Task OnAddClicked()
        {
            _DocumentTypesSelect = await UnitOfWork.Repository<DocumentType>().GetAllAsync();
            _Document = new();
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
            if (_DocumentsDetails is null)
            {
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: "No Document selected.");
                return;
            }

            try
            {
                var json = JsonSerializer.Serialize(_DocumentsDetails, new JsonSerializerOptions { WriteIndented = true });
                await JS.InvokeVoidAsync("navigator.clipboard.writeText", json);

                await NotificationService.ShowMessageBarAsync(options =>
                {
                    options.Section = MESSAGEBAR_SECTION;
                    options.Intent = MessageBarIntent.Info;
                    options.Layout = MessageBarLayout.SingleLine;
                    options.Title = $"Document Id: {_DocumentsDetails.Id} copied to clipboard!";
                    options.AllowDismiss = true;
                    options.Lifetime = TimeSpan.FromSeconds(3);
                    options.ResultTiming = MessageBarResultTiming.Closed;
                });
            }
            catch (JSException ex)
            {
                Logger.LogError(ex, "Error copying Document {Id} to clipboard.", _DocumentsDetails.Id);
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: "Could not copy the content to the clipboard.");
            }
        }

        private async Task OnDeleteClicked(int id)
        {
            var repository = UnitOfWork.Repository<Document>();
            var toDelete = await repository.GetByIdAsync(id);

            if (toDelete is null)
            {
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: $"Document Id: {id} not found.");
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
                Logger.LogError(ex, "Concurrency error while deleting Document {Id}.", id);
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: "The item was modified by another user. Refresh and try again.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error while deleting Document {Id}.", id);
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: "An error occurred while deleting the data.");
            }
        }

        private async Task OnDetailsClicked(int id)
        {
            var repository = UnitOfWork.Repository<Document>();
            var toDetails = await repository.GetByIdAsync(id);

            if (toDetails is null)
            {
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: $"Document Id: {id} not found.");
                return;
            }

            _DocumentsDetails = toDetails;
            _CollapsedDetails = false;
        }

        private async Task OnEditClicked(int id)
        {
            var repository = UnitOfWork.Repository<Document>();
            var toEdit = await repository.GetByIdAsync(id);

            if (toEdit is null)
            {
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: $"Document Id: {id} not found.");
                return;
            }

            _DocumentTypesSelect = await UnitOfWork.Repository<DocumentType>().GetAllAsync();
            _Document = toEdit;
            _CollapsedContent = false;
        }

        private async Task OnSaveContentClicked()
        {
            if (string.IsNullOrWhiteSpace(_Document.Title))
            {
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: "Title is required.");
                return;
            }

            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var repository = UnitOfWork.Repository<Document>();

            try
            {
                if (_Document.Id == 0)
                {
                    _Document.InsertAudit(authState.User);
                    await repository.AddAsync(_Document);
                }
                else
                {
                    _Document.UpdateAudit(authState.User);
                    await repository.UpdateAsync(_Document);
                }

                await UnitOfWork.SaveAsync();

                await SearchData();
                ResetForm();
                _CollapsedContent = true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Logger.LogError(ex, "Concurrency error Document Id: {Id}.", _Document.Id);
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: "Concurrency error. Cancel the operation and try again.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error Document Id: {Id}.", _Document.Id);
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: "An error occurred while saving the data.");
            }
        }

        private async Task OnSearchClicked()
        {
            await SearchData();
        }

        private void ResetForm()
        {
            _Document = new Document();
        }

        private async Task SearchData()
        {
            _SearchButtonLoading = true;

            try
            {
                var findeds = await UnitOfWork.Repository<Document>().Entities
                    .Where(d => string.IsNullOrWhiteSpace(_NameSearch) || d.Title.Contains(_NameSearch))
                    .Include(d => d.DocumentType)
                    .AsNoTracking()
                    .Take(MaxRows.MaxThirty)
                    .ToListAsync();

                if (findeds.Count >= MaxRows.MaxThirty)
                {
                    await NotificationService.ShowWarningBarAsync(MESSAGEBAR_SECTION, title: $"Your search returned more than {MaxRows.MaxThirty} results. Improve your filter.");
                }

                _Documents = findeds.AsQueryable();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error searching Documents.");
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
