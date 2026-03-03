using mobil.ViewModels;

namespace mobil;

public partial class DashboardPage : ContentPage
{
	public DashboardPage(DashboardViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		if (BindingContext is DashboardViewModel vm)
		{
			await vm.LoadData();
		}
	}
}