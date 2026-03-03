using mobil.ViewModels;

namespace mobil.Pages;

public partial class NotificationPage : ContentPage
{
	public NotificationPage(NotificationViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is NotificationViewModel vm)
            await vm.Load();
    }
}