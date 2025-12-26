using SprintManagementDashboardSample.ViewModels;

namespace SprintManagementDashboardSample.Views;

public partial class DesktopPage : ContentPage
{

    public DesktopPage() : this(Resolve<DashboardViewModel>())
    {
    }

	public DesktopPage(DashboardViewModel vm)
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