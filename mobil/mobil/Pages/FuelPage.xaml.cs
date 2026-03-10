using mobil.ViewModels;

namespace mobil.Pages;

public partial class FuelPage : ContentPage
{
	public FuelPage(FuelViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		if (BindingContext is FuelViewModel vm)
			await vm.LoadFuels();
	}
}