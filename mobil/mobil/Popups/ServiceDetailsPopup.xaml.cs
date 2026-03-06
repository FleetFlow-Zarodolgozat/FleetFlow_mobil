using CommunityToolkit.Maui.Views;
using mobil.ViewModels;

namespace mobil.Popups;

public partial class ServiceDetailsPopup : Popup
{
	public ServiceDetailsPopup(ServiceDetailsViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
        vm.CloseAction = async () => await CloseAsync();
    }
}