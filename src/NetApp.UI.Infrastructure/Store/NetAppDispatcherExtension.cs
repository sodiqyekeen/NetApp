using Fluxor;

namespace NetApp.UI.Infrastructure.Store;

public static class NetAppDispatcherExtensions
{
    public static void ChangeThemeMode(this IDispatcher dispatcher, bool isDarkMode) =>
    dispatcher.Dispatch(new ChangeThemeModeAction(isDarkMode));

    public static void ToggleLoader(this IDispatcher dispatcher, bool isLoading) =>
     dispatcher.Dispatch(new ToggleLoadingAction(isLoading));
}