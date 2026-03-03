using CommunityToolkit.Maui.Views;
using mobil.ViewModels;

namespace mobil.Popups;

public partial class ForgotPasswordPopup : Popup
{
	public ForgotPasswordPopup(ForgotPasswordViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
		var popup = this;
		vm.CloseAction = async () => await popup.CloseAsync();
	}
}