using Microsoft.Extensions.Logging;
using SprintManagementDashboard;
using Syncfusion.Maui.Core.Hosting;
using Syncfusion.Maui.Toolkit.Hosting;

namespace SprintManagementDashboard
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .ConfigureSyncfusionToolkit()
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .ConfigureSyncfusionCore();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<DashboardViewModel>();
            builder.Services.AddSingleton<MainPage>();

            return builder.Build();
        }
    }
}
