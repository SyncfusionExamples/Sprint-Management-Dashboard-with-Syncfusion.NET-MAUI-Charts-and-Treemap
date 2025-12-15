using SprintManagementDashboardSample.ViewModels;

namespace SprintManagementDashboardSample.Views;

public partial class MobilePage : ContentPage
{
    public MobilePage() : this(Resolve<DashboardViewModel>())
    {
    }

    public MobilePage(DashboardViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    private static T Resolve<T>() where T : class, new()
    {
        var services = Application.Current?.Handler?.MauiContext?.Services;
        return services?.GetService(typeof(T)) as T ?? new T();
    }
}
