using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetApp.UI.Components;

public partial class NetAppDialog : NetAppComponentBase
{
    private DialogParameters _parameters = default!;
    private bool _hidden;

    [CascadingParameter]
    private BaseDialogContext? DialogContext { get; set; } = default!;

    [Parameter]
    public DialogInstance Instance { get; set; } = default!;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public EventCallback<DialogResult> OnDialogResult { get; set; }

    [Parameter] public EventCallback<bool> HiddenChanged { get; set; }

    [Parameter]
    public bool Hidden
    {
        get => _hidden;
        set
        {
            if (value == _hidden)
            {
                return;
            }

            _hidden = value;
            HiddenChanged.InvokeAsync(value);
        }
    }

    public async Task CancelAsync() => await CloseAsync(DialogResult.Cancel());

    /// <summary>
    /// Closes the dialog with a cancel result.
    /// </summary>
    /// <param name="returnValue"></param>
    /// <returns></returns>
    public async Task CancelAsync<T>(T returnValue) => await CloseAsync(DialogResult.Cancel(returnValue));

    /// <summary>
    /// Closes the dialog with a OK result.
    /// </summary>
    /// <returns></returns>
    public async Task CloseAsync() => await CloseAsync(DialogResult.Ok<object?>(null));

    /// <summary>
    /// Closes the dialog with a OK result.
    /// </summary>
    /// <param name="returnValue"></param>
    /// <returns></returns>
    public async Task CloseAsync<T>(T returnValue) => await CloseAsync(DialogResult.Ok(returnValue));

    /// <summary>
    /// Closes the dialog
    /// </summary>
    public async Task CloseAsync(DialogResult dialogResult)
    {
        DialogContext?.DialogContainer.DismissInstance(Id!, dialogResult);
        if (Instance is not null)
        {
            if (Instance.Parameters.OnDialogResult.HasDelegate)
            {
                await Instance.Parameters.OnDialogResult.InvokeAsync(dialogResult);
            }
        }
        else
        {
            Hide();
        }
    }

    public void Hide()
    {
        Hidden = true;
       // RefreshHeaderFooter();
    }
}
