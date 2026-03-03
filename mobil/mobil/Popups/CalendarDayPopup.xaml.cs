using CommunityToolkit.Maui.Views;
using mobil.ViewModels;

namespace mobil.Popups;

public partial class CalendarDayPopup : Popup
{
	public CalendarDayPopup(CalendarDayViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
		vm.CloseAction = async () => await CloseAsync();
	}
}