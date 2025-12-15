using Microsoft.Extensions.DependencyInjection;
using SprintManagementDashboardSample.Views;

namespace SprintManagementDashboardSample
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var isDesktop = DeviceInfo.Idiom == DeviceIdiom.Desktop || DeviceInfo.Idiom == DeviceIdiom.TV;
            Page root = isDesktop
                ? new DesktopPage()
                : new MobilePage();

            return new Window(root);
        }
    }
}