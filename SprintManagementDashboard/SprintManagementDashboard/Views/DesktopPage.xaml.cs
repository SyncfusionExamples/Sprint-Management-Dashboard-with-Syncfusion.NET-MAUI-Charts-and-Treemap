namespace SprintManagementDashboard;

public partial class DesktopPage : ContentPage
{
	public DesktopPage()
	{
		InitializeComponent();
    }

    private static T Resolve<T>() where T : class, new()
    {
        var services = Application.Current?.Handler?.MauiContext?.Services;
        return services?.GetService(typeof(T)) as T ?? new T();
    }
}