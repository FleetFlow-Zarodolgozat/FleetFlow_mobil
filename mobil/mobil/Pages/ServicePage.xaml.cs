using mobil.ViewModels;

namespace mobil.Pages;

public partial class ServicePage : ContentPage
{
	public ServicePage(ServiceViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ServiceViewModel vm)
            await vm.LoadServices();
    }
}