using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;

namespace MediafulTest
{
    public static class TestContextExtensions
    {
        public static void AddTestServices(this TestContext ctx)
        {
            ctx.JSInterop.Mode = JSRuntimeMode.Loose;
            ctx.Services.AddMudServices(options =>
            {
                options.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
                options.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopRight;
            });
            ctx.Services.AddScoped(sp => new HttpClient());
            ctx.Services.AddOptions();
        }
    }
}
