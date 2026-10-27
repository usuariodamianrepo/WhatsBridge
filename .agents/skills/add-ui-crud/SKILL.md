---
name: add-ui-crud
description: Create a UI CRUD from a specific `Entity` of `src/Clone.Models`. Save the CRUD page at `src/Clone.Web.Components.Pages` folder. Use the Templates, replace the [Entity] name in the template to corresponding definition, if {EntitySingularName} you write the entity name at singular name and if {EntityPluralName} you write the entity name at plural name. Create the UI markup and the code to support it (Code-Behind). If see {SPECIAL_CASE: } implemented the code descript.
argument-hint: "[EntityName]"
---

# Templates

## Template UI markup
Create into de folder `src/Clone.Web.Components.Pages/{EntityPluralName}.razor` file with the follow code
```csharp
@page "/{EntityPluralNameLowerCase}"
@rendermode InteractiveServer

@using Clone.DataAccess.Repositories.IRepository
@using Clone.Utility
@using Microsoft.AspNetCore.Authorization
@using Microsoft.AspNetCore.Components.Authorization

@inject AuthenticationStateProvider AuthenticationStateProvider
@inject IJSRuntime JS
@inject ILogger<{EntityPluralName}> Logger
@inject INotificationService NotificationService
@inject IUnitOfWork UnitOfWork

@attribute [Authorize(Roles = RD.RoleAdmin)]

<PageTitle>{EntityPluralName}</PageTitle>
<FluentDialogProvider />
<FluentLabel Size="@LabelSize.Large">{EntityPluralName}</FluentLabel>
<FluentMessageBarProvider Section="@MESSAGEBAR_SECTION" Margin="@Margin.Vertical2" />

<FluentKeyCode OnKeyDown="@KeyDownHandler">
    <FluentStack HorizontalGap="6px" Wrap="true">
        <FluentButton Title="@SharedLocalizer["Common.Add"]" IconStart="@(new Icons.Regular.Size20.Add())" Appearance="ButtonAppearance.Default"
                      OnClick="@OnAddClicked"/>
        <FluentTextInput @bind-Value=_NameSearch Placeholder="Name" TextInputType="TextInputType.Search" Maxlength=10 AutoComplete="off" Autofocus="true" />
        <FluentButton Title="@SharedLocalizer["Common.Search"]" IconStart="@(new Icons.Regular.Size20.DatabaseSearch())" Appearance="ButtonAppearance.Primary"
                      OnClick="@OnSearchClicked" Loading="@_SearchButtonLoading">@SharedLocalizer["Common.Search"]</FluentButton>
    </FluentStack>
</FluentKeyCode>

<FluentMultiSplitter>

    <FluentMultiSplitterPane Size="50%">
        <FluentDataGrid @ref="_Grid" Items=@_{EntityPluralName} GridTemplateColumns="1fr 1fr 1fr 1fr">
            <ChildContent>
				{SPECIAL_CASE: for each entity property repeat this little code with the property name `<PropertyColumn Property="@(c => c.PropertyName)"` /> }
                <TemplateColumn Title="@SharedLocalizer["Common.Actions"]" Align="@DataGridCellAlignment.Center">
                    <FluentButton Title="@SharedLocalizer["Common.Edit"]" Appearance="ButtonAppearance.Subtle" IconEnd="@(new Icons.Regular.Size20.Edit())" OnClick="@(() => OnEditClicked(context.Id))" />
                    <FluentButton Title="@SharedLocalizer["Common.Details"]" Appearance="ButtonAppearance.Subtle" IconEnd="@(new Icons.Regular.Size20.TextDescription())" OnClick="@(() => OnDetailsClicked(context.Id))" />
                    <FluentButton Title="@SharedLocalizer["Common.Delete"]" Appearance="ButtonAppearance.Subtle" IconEnd="@(new Icons.Regular.Size20.Delete().WithColor(Color.Error))" OnClick="@(() => OnDeleteClicked(context.Id))" />
                </TemplateColumn>
            </ChildContent>
            <EmptyContent>
                <FluentStack Orientation="Orientation.Horizontal" VerticalAlignment="VerticalAlignment.Center" HorizontalAlignment="HorizontalAlignment.Center" HorizontalGap="10">
                    <FluentIcon Value="@(new Icons.Filled.Size24.Crown())" Color="@Color.Primary" />
                    <div>@SharedLocalizer["Message.NoData"]</div>
                </FluentStack>
            </EmptyContent>
        </FluentDataGrid>
    </FluentMultiSplitterPane>

    <FluentMultiSplitterPane Collapsed="_CollapsedContent" Size="25%">
        <FluentLabel Size="@LabelSize.Large">Content - Id: @(_{EntitySingularName}?.Id)</FluentLabel>
        <FluentDivider />
        @if (_{EntitySingularName} != null)
        {
            <EditForm Model="_{EntitySingularName}" OnValidSubmit="@(async () => await OnSaveContentClicked())">
                <DataAnnotationsValidator />
                <FluentGrid Justify="JustifyContent.FlexStart" Spacing="1" Padding="3px">
				{SPECIAL_CASE: for each entity property repeat this little code with the property name.
				IF Property is string THEN
                    `<FluentGridItem Xs="12" Sm="12">
                        <FluentTextInput Label="PropertyName" @bind-Value=_{EntitySingularName}.PropertyName />
                    </FluentGridItem>`
				ELSE IF Property is int THEN
                    `<FluentGridItem Xs="12" Sm="12">
                        <FluentTextInput Label="PropertyName" @bind-Value=_{EntitySingularName}.PropertyName TextInputType="TextInputType.Number"/>
                    </FluentGridItem>`
				ELSE IF Property is DateTime THEN
				    `<FluentGridItem Xs="12" Sm="12">
                        <FluentDatePicker Label="PropertyName" @bind-Value=_{EntitySingularName}.PropertyName />
                    </FluentGridItem>`
				ELSE IF Property is bool THEN
					`<FluentGridItem Xs="12" Sm="12">
                        <FluentCheckbox Label="PropertyName" @bind-Value=_{EntitySingularName}.PropertyName />
                    </FluentGridItem>`
				ELSE IF Property is ForeignKey THEN
					`<FluentGridItem Xs="12" Sm="12">
						<FluentSelect Label="PropertyName"
							  Placeholder="Select a PropertyName"
							  Items="@_{PropertyName}Select"
							  @bind-Value="@_{EntitySingularName}.PropertyName" />
                    </FluentGridItem>`
				}
                </FluentGrid>
                <FluentStack id="toolbar-footer" HorizontalGap="6px" Padding="3px">
                    <FluentButton Type="ButtonType.Submit" Title="@SharedLocalizer["Common.Save"]" IconStart="@(new Icons.Regular.Size20.Save())" 									
						Appearance="ButtonAppearance.Primary">@SharedLocalizer["Common.Save"]</FluentButton>
                    <FluentButton Title="Cancel" IconStart="@(new Icons.Regular.Size20.ArrowExit())" Appearance="ButtonAppearance.Default"
                        OnClick="OnCancelContentClicked">Cancel</FluentButton>
                </FluentStack>
            </EditForm>
        }
        else
        {
            <p>@SharedLocalizer["Message.NoSelection"]</p>
        }
    </FluentMultiSplitterPane>

    <FluentMultiSplitterPane Collapsed="_CollapsedDetails" Size="25%">
        <FluentLabel Size="@LabelSize.Large">Details - Id: @(_{EntityPluralName}Details?.Id)</FluentLabel>
        <FluentDivider />
        @if (_{EntityPluralName}Details != null)
        {
            <FluentGrid Justify="JustifyContent.FlexStart" Spacing="1" Padding="3px">
			{SPECIAL_CASE: for each entity property repeat this little code with the property name
                <FluentGridItem Xs="12" Sm="12">
                    <b>PropertyName:</b> @(_{EntitySingularName}.PropertyName)
                </FluentGridItem>`
                <FluentDivider AlignContent="DividerAlignContent.Center">
                    Audit Information
                </FluentDivider>
                <FluentGridItem Xs="12">
                    <b>InsertDate:</b> @(_{EntityPluralName}Details.InsertDate.ConvertToShow().ToString("yyyy-MM-dd HH:mm:ss"))
                </FluentGridItem>
                <FluentGridItem Xs="12">
                    <b>InsertUser:</b> @(_{EntityPluralName}Details.InsertUser)
                </FluentGridItem>
                <FluentGridItem Xs="12">
                    <b>UpdateDate:</b> @(_{EntityPluralName}Details.UpdateDate?.ConvertToShow().ToString("yyyy-MM-dd HH:mm:ss"))
                </FluentGridItem>
                <FluentGridItem Xs="12">
                    <b>UpdateUser:</b> @(_{EntityPluralName}Details.UpdateUser)
                </FluentGridItem>
			}
            </FluentGrid>
            <br />
            <FluentStack id="toolbar-footer" HorizontalGap="6px">
                <FluentButton Title="Copy" IconStart="@(new Icons.Regular.Size20.Copy())" Appearance="ButtonAppearance.Default"
                              OnClick="OnCopyDetailsToClipboard">Copy</FluentButton>
                <FluentButton Title="@SharedLocalizer["Common.Cancel"]" IconStart="@(new Icons.Regular.Size20.ArrowExit())" Appearance="ButtonAppearance.Default"
                              OnClick="OnCancelDetailsClicked">@SharedLocalizer["Common.Cancel"]</FluentButton>
            </FluentStack>
        }
        else
        {
            <p>@SharedLocalizer["Message.NoSelection"]</p>
        }
    </FluentMultiSplitterPane>

</FluentMultiSplitter>
```

## Template Code-Behind
Create into de folder `src/Clone.Web.Components.Pages/{EntityPluralName}.razor.cs` file with the follow code
```csharp
using Clone.Models;
using Clone.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;

namespace Clone.Web.Components.Pages
{
    public partial class {EntityPluralName}
    {
        private const string MESSAGEBAR_SECTION = "MESSAGEBAR_{EntitySingularNameUpperCase}";
        private FluentDataGrid<{EntitySingularName}>? _Grid;

        private {EntitySingularName} _{EntitySingularName} { get; set; } = new();
        private {EntitySingularName} _{EntityPluralName}Details { get; set; } = new();
        private IQueryable<{EntitySingularName}>? _{EntityPluralName} { get; set; }

		{SPECIAL_CASE: for each entity property repeat this little code with the property name
		IF Property is ForeignKey THEN
		`IEnumerable<{PropertyName}> _{PropertyName}Select = default!;`
		}

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
            _{EntitySingularName} = new();
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
            if (_{EntityPluralName}Details is null)
            {
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: "No {EntitySingularName} selected.");
                return;
            }

            try
            {
                var json = JsonSerializer.Serialize(_{EntityPluralName}Details, new JsonSerializerOptions { WriteIndented = true });
                await JS.InvokeVoidAsync("navigator.clipboard.writeText", json);

                await NotificationService.ShowMessageBarAsync(options =>
                {
                    options.Section = MESSAGEBAR_SECTION;
                    options.Intent = MessageBarIntent.Info;
                    options.Layout = MessageBarLayout.SingleLine;
                    options.Title = $"{EntitySingularName} Id: {_{EntityPluralName}Details.Id} copied to clipboard!";
                    options.AllowDismiss = true;
                    options.Lifetime = TimeSpan.FromSeconds(3);
                    options.ResultTiming = MessageBarResultTiming.Closed;
                });
            }
            catch (JSException ex)
            {
                Logger.LogError(ex, "Error copying {EntitySingularName} {Id} to clipboard.", _{EntityPluralName}Details.Id);
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: "Could not copy the content to the clipboard.");
            }
        }

        private async Task OnDeleteClicked(int id)
        {
            var repository = UnitOfWork.Repository<{EntitySingularName}>();
            var toDelete = await repository.GetByIdAsync(id);

            if (toDelete is null)
            {
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: $"{EntitySingularName} Id: {id} not found.");
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
                Logger.LogError(ex, "Concurrency error while deleting {EntitySingularName} {Id}.", id);
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: "The item was modified by another user. Refresh and try again.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error while deleting {EntitySingularName} {Id}.", id);
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: "An error occurred while deleting the data.");
            }
        }

        private async Task OnDetailsClicked(int id)
        {
            var repository = UnitOfWork.Repository<{EntitySingularName}>();
            var toDetails = await repository.GetByIdAsync(id);

            if (toDetails is null)
            {
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: $"{EntitySingularName} Id: {id} not found.");
                return;
            }

            _{EntityPluralName}Details = toDetails;
            _CollapsedDetails = false;
        }

        private async Task OnEditClicked(int id)
        {
            var repository = UnitOfWork.Repository<{EntitySingularName}>();
            var toEdit = await repository.GetByIdAsync(id);

            if (toEdit is null)
            {
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: $"{EntitySingularName} Id: {id} not found.");
                return;
            }

		{SPECIAL_CASE: for each entity property repeat this little code with the property name
		IF Property is ForeignKey THEN
			`_{PropertyName}Select = await _UnitOfWork.Repository<{PropertyName}>().GetAllAsync();`
		}
            _{EntitySingularName} = toEdit;
            _CollapsedContent = false;
        }

        private async Task OnSaveContentClicked()
        {
            if (string.IsNullOrWhiteSpace(_{EntitySingularName}.Name))
            {
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: "Name is required.");
                return;
            }

            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var repository = UnitOfWork.Repository<{EntitySingularName}>();

            try
            {
                if (_{EntitySingularName}.Id == 0)
                {
                    _{EntitySingularName}.InsertAudit(authState.User);
                    await repository.AddAsync(_{EntitySingularName});
                }
                else
                {
                    _{EntitySingularName}.UpdateAudit(authState.User);
                    await repository.UpdateAsync(_{EntitySingularName});
                }

                await UnitOfWork.SaveAsync();

                await SearchData();
                ResetForm();
                _CollapsedContent = true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Logger.LogError(ex, "Concurrency error {EntitySingularName} Id: {Id}.", _{EntitySingularName}.Id);
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: "Concurrency error. Cancel the operation and try again.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error {EntitySingularName} Id: {Id}.", _{EntitySingularName}.Id);
                await NotificationService.ShowErrorBarAsync(MESSAGEBAR_SECTION, title: "An error occurred while saving the data.");
            }
        }

        private async Task OnSearchClicked()
        {
            await SearchData();
        }

        private void ResetForm()
        {
            _{EntitySingularName} = new {EntitySingularName}();
        }

        private async Task SearchData()
        {
            _SearchButtonLoading = true;

            try
            {
			    var findeds = await UnitOfWork.Repository<{EntitySingularName}>().Entities
                    .Where(d => string.IsNullOrWhiteSpace(_NameSearch) || d.Name.Contains(_NameSearch))
                    .AsNoTracking()
                    .Take(MaxRows.MaxThirty)
                    .ToListAsync();

                if (findeds.Count >= MaxRows.MaxThirty)
                {
                    await NotificationService.ShowWarningBarAsync(MESSAGEBAR_SECTION, title: $"Your search returned more than {MaxRows.MaxThirty} results. Improve your filter.");
                }

                _{EntityPluralName} = findeds.AsQueryable();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error searching {EntityPluralName}.");
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
```