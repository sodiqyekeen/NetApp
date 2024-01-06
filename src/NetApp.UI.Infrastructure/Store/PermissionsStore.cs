using Fluxor;

namespace NetApp.UI.Infrastructure.Store;

public record PermissionsState(
    List<ModulePermission> CurrentPermissions,
    List<ModulePermission> OriginalPermissions)
{
    public bool IsDirty => !OriginalPermissions.SequenceEqual(CurrentPermissions);
}

public record InitializePermissionsAction(List<ModulePermission> Permissions);
public record UpdatePermissionsAction(List<ModulePermission> Permissions);
public record ResetPermissionsAction();

public class PermissionsFeature : Feature<PermissionsState>
{
    public override string GetName() => nameof(PermissionsState).ToLower();
    protected override PermissionsState GetInitialState() => new([], []);
}

public class PermissionsReducer
{
    [ReducerMethod]
    public static PermissionsState ReduceInitializePermissionsAction(PermissionsState _, InitializePermissionsAction action) =>
        new(action.Permissions, ClonePermissions(action.Permissions).ToList());

    [ReducerMethod]
    public static PermissionsState ReduceUpdatePermissionsAction(PermissionsState state, UpdatePermissionsAction action) =>
        state with { CurrentPermissions = action.Permissions };

    [ReducerMethod]
    public static PermissionsState ReduceResetPermissionsAction(PermissionsState __, ResetPermissionsAction _) =>
        new([], []);

    private static IEnumerable<ModulePermission> ClonePermissions(IEnumerable<ModulePermission> permissions)
    {
        return permissions.Select(mp => mp with { Permissions = mp.Permissions.Select(p => p with { }).ToList() });
    }
}