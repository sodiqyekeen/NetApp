using Fluxor;

namespace NetApp.UI.Infrastructure.Store;

public static class Reducers
{
    [ReducerMethod]
    public static NetAppState ReduceChangeThemeModeAction(NetAppState state, ChangeThemeModeAction action) =>
     state with { IsDarkMode = action.IsDarkMode };

    [ReducerMethod]
    public static NetAppState ReduceIncrementCountAction(NetAppState state, IncrementCountAction _) =>
        state with { Count = state.Count + 1 };

    [ReducerMethod]
    public static NetAppState ReduceInitializeAppStateAction(NetAppState _, InitializeAppStateAction action) =>
        action.AppState;


    [ReducerMethod]
    public static NetAppState ReduceToggleLoadingAction(NetAppState state, ToggleLoadingAction action) =>
        state.IsLoading == action.IsLoading ? state : (state with { IsLoading = action.IsLoading });

    [ReducerMethod]
    public static NetAppState ReduceSetCurrentUserDetailAction(NetAppState state, SetCurrentUserDetailAction action) =>
        state with { CurrentUser = action.User };
}