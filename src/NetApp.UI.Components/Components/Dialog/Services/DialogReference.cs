namespace NetApp.UI.Components;

public class DialogReference(string dialogInstanceId, IDialogService dialogService) : IDialogReference
{
    private readonly TaskCompletionSource<DialogResult> _resultCompletion = new();

    public DialogInstance Instance { get; set; } = default!;

    public string Id { get; } = dialogInstanceId;

    public Task<DialogResult> Result => _resultCompletion.Task;

    public async Task CloseAsync() => await dialogService.CloseAsync(this);

    public async Task CloseAsync(DialogResult result) => await dialogService.CloseAsync(this, result);

    public virtual bool Dismiss(DialogResult result)
    {
        _resultCompletion.TrySetResult(result);

        return true;
    }

    public async Task<T?> GetReturnValueAsync<T>()
    {
        DialogResult? result = await Result;
        try
        {
            if (result.Data == null)
            {
                return default;
            }
            else
            {
                return (T)result.Data ?? default;
            }
        }
        catch (InvalidCastException)
        {
            return default;
        }
    }
}

