using System.Text.Json.Serialization;
using Fluxor;

namespace NetApp.UI.Infrastructure.Store;

public record NetAppState(
    bool IsDarkMode,
    int Count,
    [property: JsonIgnore] bool IsLoading = false
    );

public record ChangeThemeModeAction(bool IsDarkMode);

public record IncrementCountAction();

public record InitializeAppStateAction(NetAppState AppState);

public record ToggleLoadingAction(bool IsLoading);
