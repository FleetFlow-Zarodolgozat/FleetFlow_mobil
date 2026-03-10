using mobil.ViewModels;

namespace mobil.Pages;

public partial class TripPage : ContentPage
{
	public TripPage(TripViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is TripViewModel vm)
            await vm.LoadTrips();
    }
}