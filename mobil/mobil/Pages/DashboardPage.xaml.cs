using mobil.ViewModels;

namespace mobil;

public partial class DashboardPage : ContentPage
{
	public DashboardPage(DashboardViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }
}