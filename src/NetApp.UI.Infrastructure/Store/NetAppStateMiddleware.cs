using Fluxor;

namespace NetApp.UI.Infrastructure.Store;

public class NetAppStateMiddleware : Middleware
{
    private readonly IStorageService _storageService;
    private IStore? _store;
    private Task TailTask = Task.CompletedTask;

    public NetAppStateMiddleware(IStorageService storageService)
    {
        _storageService = storageService;
    }

    public override async Task InitializeAsync(IDispatcher dispatcher, IStore store)
    {
        _store = store;
        var existingState = await _storageService.GetItemAsync<NetAppState>(ApplicationConstants.Storage.AppState);
        if (existingState is not null)
            dispatcher.Dispatch(new InitializeAppStateAction(existingState));
    }

    public override void AfterDispatch(object action)
    {
        var state = GetAppState();
        if (state != null)
            TailTask = TailTask.ContinueWith(_ => _storageService.SetItemAsync(ApplicationConstants.Storage.AppState, state)).Unwrap();
    }
    private IDictionary<string, object> GetState()
    {
        var state = new Dictionary<string, object>();
        foreach (IFeature feature in _store!.Features.Values)
            state[feature.GetName()] = feature.GetState();
        return state;
    }

    private NetAppState? GetAppState()
    {
        return _store!.Features.Values
        .Where(f => f.GetName() == nameof(NetAppState).ToLower())
        .FirstOrDefault()?.GetState() as NetAppState;
    }
}