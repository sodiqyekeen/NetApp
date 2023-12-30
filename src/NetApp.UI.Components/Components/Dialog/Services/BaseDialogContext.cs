using System.Collections.ObjectModel;

namespace NetApp.UI.Components;

internal sealed class BaseDialogContext(NetAppDialogProvider container)
{
    public Collection<IDialogReference> References { get; set; } = [];
    public NetAppDialogProvider DialogContainer { get; } = container;
}
