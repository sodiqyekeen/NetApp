using Fluxor;
namespace NetApp.UI.Infrastructure.Store;

public class NetAppFeature : Feature<NetAppState>
{
    public override string GetName() => nameof(NetAppState).ToLower();
    protected override NetAppState GetInitialState() => new(false,0);
}