using System.ComponentModel;
using System.Reflection;
using NetApp.Application.Dtos.Identity;
using NetApp.Shared.Constants;

namespace NetApp.Infrastructure.Identity;

public static class PermissionHelper
{
    public static IEnumerable<ModulePermission> GetAllPermissions()
    {
        foreach (var module in typeof(Permissions).GetNestedTypes())
        {
            var permissions = GetAllPermissionsByModule(module).ToList();
            yield return new ModulePermission(
                module.GetCustomAttribute<DisplayNameAttribute>()!.DisplayName,
                module.GetCustomAttribute<DescriptionAttribute>()!.Description,
                permissions);
        }
    }

    private static IEnumerable<Permission> GetAllPermissionsByModule(Type module)
    {
        var fields = module.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
        foreach (var field in fields)
        {
            yield return new Permission(field.Name, GetPermissionValueFromField(field), field.GetCustomAttribute<DescriptionAttribute>()!.Description);
        }
    }

    private static string GetPermissionValueFromField(FieldInfo field) => field.GetValue(null)?.ToString() ?? string.Empty;
}