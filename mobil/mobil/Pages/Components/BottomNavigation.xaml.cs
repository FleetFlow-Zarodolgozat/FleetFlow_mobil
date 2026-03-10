using mobil.ViewModels;

namespace mobil.Pages.Components;

public partial class BottomNavigation : ContentView
{
	public static readonly BindableProperty ActiveTabProperty = BindableProperty.Create(nameof(ActiveTab), typeof(string), typeof(BottomNavigation), string.Empty);

	public string ActiveTab
	{
		get => (string)GetValue(ActiveTabProperty);
		set => SetValue(ActiveTabProperty, value);
	}

	public BottomNavigation()
	{
		InitializeComponent();
		BindingContext = IPlatformApplication.Current!.Services.GetRequiredService<BottomNavigationViewModel>();
	}
}