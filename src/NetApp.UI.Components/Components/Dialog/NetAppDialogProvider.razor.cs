using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace NetApp.UI.Components;

public partial class NetAppDialogProvider : IDisposable
{
    private readonly BaseDialogContext _dialogContext;
    private readonly RenderFragment _renderDialogs;

    [Inject] private IDialogService DialogService { get; set; } = default!;
    [Inject] public NavigationManager NavigationManager { get; set; } = default!;

    public NetAppDialogProvider()
    {
        _dialogContext = new(this);
        _renderDialogs = RenderDialogs;

    }
    protected override void OnInitialized()
    {
        NavigationManager.LocationChanged += LocationChanged;

        DialogService.OnShow += ShowDialog;
        DialogService.OnShowAsync += ShowDialogAsync;
        DialogService.OnUpdate += UpdateDialog;
        DialogService.OnUpdateAsync += UpdateDialogAsync;
        DialogService.OnDialogCloseRequested += DismissInstance;
    }
    internal IDialogReference? GetDialogReference(string id) =>
        _dialogContext.References.SingleOrDefault(x => x.Id == id);

    private void ShowDialog(IDialogReference dialogReference, Type? dialogComponent, DialogParameters parameters, object content)
    {
        DialogInstance dialog = new(dialogComponent, parameters, content);
        dialogReference.Instance = dialog;

        _dialogContext.References.Add(dialogReference);
        InvokeAsync(StateHasChanged);
    }

    private async Task<IDialogReference> ShowDialogAsync(IDialogReference dialogReference, Type? dialogComponent, DialogParameters parameters, object content)
    {
        return await Task.Run(() =>
        {
            DialogInstance dialog = new(dialogComponent, parameters, content);
            dialogReference.Instance = dialog;

            _dialogContext.References.Add(dialogReference);
            InvokeAsync(StateHasChanged);

            return dialogReference;
        });
    }

    private void UpdateDialog(string? dialogId, DialogParameters parameters)
    {
        IDialogReference reference = _dialogContext.References.SingleOrDefault(x => x.Id == dialogId)!;
        DialogInstance? dialogInstance = reference.Instance;

        if (dialogInstance is not null)
        {
            dialogInstance.Parameters = parameters;

            InvokeAsync(StateHasChanged);
        };
    }

    private async Task<IDialogReference?> UpdateDialogAsync(string? dialogId, DialogParameters parameters)
    {
        return await Task.Run(() =>
        {
            IDialogReference? reference = _dialogContext.References.SingleOrDefault(x => x.Id == dialogId)!;
            DialogInstance? dialogInstance = reference?.Instance;

            if (dialogInstance is not null)
            {
                dialogInstance.Parameters = parameters;

                InvokeAsync(StateHasChanged);
            }
            return reference;
        });
    }

    internal void DismissInstance(string id, DialogResult result)
    {
        IDialogReference? reference = GetDialogReference(id);
        if (reference is not null)
        {
            DismissInstance(reference, result);
        }
    }

    public void DismissAll()
    {
        _dialogContext.References.ToList().ForEach(r => DismissInstance(r, DialogResult.Cancel()));
        StateHasChanged();
    }

    private void DismissInstance(IDialogReference dialog, DialogResult result)
    {
        if (!dialog.Dismiss(result))
        {
            return;
        }

        _dialogContext.References.Remove(dialog);
        StateHasChanged();
    }

    private void LocationChanged(object? sender, LocationChangedEventArgs args)
    {
        DismissAll();
    }

    private void ClearAll()
    {
        _dialogContext.References.Clear();
    }

    private async Task OnDismissAsync(DialogEventArgs args)
    {
        if (args is not null && args.Reason is not null && args.Reason == "dismiss" && !string.IsNullOrWhiteSpace(args.Id))
        {
            IDialogReference? dialog = GetDialogReference(args.Id);
            if (dialog!.Instance.Parameters.PreventDismissOnOverlayClick == false)
            {
                await dialog!.CloseAsync(DialogResult.Cancel());
            }
        }
    }

    public void Dispose()
    {
        if (NavigationManager != null)
        {
            NavigationManager.LocationChanged -= LocationChanged;
        }
    }
}
