using Microsoft.AspNetCore.Components;

namespace NetApp.Pwa.Shared;

public partial class UnauthorizedLayout
{
    [Parameter] public RenderFragment ChildContent { get; set; } = null!;
}