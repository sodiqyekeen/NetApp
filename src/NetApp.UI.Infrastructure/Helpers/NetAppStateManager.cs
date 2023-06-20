namespace NetApp.UI.Infrastructure;
public class NetAppStateManager
{
    public event Action? OnChange;
    private void NotifyStateChanged() => OnChange?.Invoke();
}
