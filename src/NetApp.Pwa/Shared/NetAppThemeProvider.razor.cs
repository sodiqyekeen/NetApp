using Fluxor;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using NetApp.UI.Infrastructure.Store;

namespace NetApp.Pwa.Shared;

public partial class NetAppThemeProvider
{
    [Inject] private IState<NetAppState> AppState { get; set; } = null!;
    [Inject] private IDispatcher Dispatcher { get; set; } = null!;

    private MudThemeProvider _themeProvider = new();

    private readonly MudTheme _theme = new()
    {
        Palette = new PaletteLight
        {
            Primary = new MudBlazor.Utilities.MudColor("#009ef7"),
            Secondary = new MudBlazor.Utilities.MudColor("#E4E6EF"),
            Success = new MudBlazor.Utilities.MudColor("#50cd89"),
            Info = new MudBlazor.Utilities.MudColor("#7239ea"),
            Warning = new MudBlazor.Utilities.MudColor("#ffc700"),
            Error = new MudBlazor.Utilities.MudColor("#f1416c"),
            Dark = new MudBlazor.Utilities.MudColor("#181C32"),
            SuccessLighten = "#e8fff3",
            PrimaryLighten = "#f1faff",
            SecondaryLighten = "#f5f8fa",
            InfoLighten = "#f8f5ff",
            WarningLighten = "#fff8dd",
            ErrorLighten = "#fff5f8",
            DarkLighten = "#eff2f5",
            AppbarText = new MudBlazor.Utilities.MudColor("#A1A5B7"),
            AppbarBackground = new MudBlazor.Utilities.MudColor("#ffffff"),
            Background = new MudBlazor.Utilities.MudColor("#f5f8fa"),
            DrawerBackground = new MudBlazor.Utilities.MudColor("#1e1e2d"),
            TextDisabled = new MudBlazor.Utilities.MudColor("#A1A5B7"),
            LinesDefault = new MudBlazor.Utilities.MudColor("#eff2f5"),
            TextPrimary = new MudBlazor.Utilities.MudColor("#181C32"),
            TextSecondary = new MudBlazor.Utilities.MudColor("#E4E6EF"),
            DrawerText = new MudBlazor.Utilities.MudColor("#FFFFFF"),
            
        },
        PaletteDark = new PaletteDark
        {
            Primary = new MudBlazor.Utilities.MudColor("#009ef7"),
            Secondary = new MudBlazor.Utilities.MudColor("#E4E6EF"),
            Success = new MudBlazor.Utilities.MudColor("#50cd89"),
            Info = new MudBlazor.Utilities.MudColor("#7239ea"),
            Warning = new MudBlazor.Utilities.MudColor("#ffc700"),
            Error = new MudBlazor.Utilities.MudColor("#f1416c"),
            Dark = new MudBlazor.Utilities.MudColor("#FFFFFF"),
            SuccessLighten = "#1c3238",
            PrimaryLighten = "#212e48",
            SecondaryLighten = "#1b1b29",
            InfoLighten = "#2f264f",
            WarningLighten = "#392f28",
            ErrorLighten = "#3a2434",
            DarkLighten = "#2B2B40",
            AppbarText = new MudBlazor.Utilities.MudColor("#565674"),
            AppbarBackground = new MudBlazor.Utilities.MudColor("#1E1E2D"),
            DrawerBackground = new MudBlazor.Utilities.MudColor("#1e1e2d"),
            Background = new MudBlazor.Utilities.MudColor("#151521"),
            TextDisabled = new MudBlazor.Utilities.MudColor("#565674"),
            LinesDefault = new MudBlazor.Utilities.MudColor("#2B2B40"),
            DrawerText = new MudBlazor.Utilities.MudColor("#FFFFFF"),
            TextPrimary=new MudBlazor.Utilities.MudColor("#ffffffe0"),
        },
        LayoutProperties = new LayoutProperties
        {
            DrawerWidthLeft = "200px",
            AppbarHeight = "65px",
        },
        Typography = new Typography
        {
            H1 = new H1
            {
                FontSize = "calc(1.3rem + .6vw)",
                FontWeight = 600,
                LineHeight = 1.2,
            },
            H2 = new H2
            {
                FontSize = "calc(1.275rem + .3vw)",
                FontWeight = 600,
                LineHeight = 1.2,
            },
            H3 = new H3
            {
                FontSize = "calc(1.26rem + .12vw)",
                FontWeight = 600,
                LineHeight = 1.2,
            },
            H4 = new H4
            {
                FontSize = "1.25rem",
                FontWeight = 600,
                LineHeight = 1.2,
            },
            H5 = new H5
            {
                FontSize = "1.125rem",
                FontWeight = 600,
                LineHeight = 1.2,
            },
            H6 = new H6
            {
                FontSize = "1.075rem",
                FontWeight = 600,
                LineHeight = 1.2,
            },
            Body1 = new Body1
            {
                FontSize = "1rem",
            },
        }
    };

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var isDarkMode = await _themeProvider.GetSystemPreference();
            Dispatcher.Dispatch(new ChangeThemeModeAction(isDarkMode));
        }
    }

}