using Microsoft.Extensions.DependencyInjection;
using mobil.Services;

namespace mobil
{
    public partial class App : Application
    {
        private readonly SessionService _sessionService;
        private readonly ThemeService _themeService;

        public App(SessionService sessionService, ThemeService themeService)
        {
            _sessionService = sessionService;
            _themeService = themeService;
            InitializeComponent();
            _themeService.ApplySavedTheme();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        protected override async void OnStart()
        {
            base.OnStart();
            var token = await _sessionService.GetToken();
            if (!string.IsNullOrEmpty(token))
                await Shell.Current.GoToAsync("//DashboardPage");
            else
                await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}