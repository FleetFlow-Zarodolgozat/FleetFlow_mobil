using Microsoft.Extensions.DependencyInjection;
using mobil.Services;

namespace mobil
{
    public partial class App : Application
    {
        private readonly SessionService _sessionService;

        public App(SessionService sessionService)
        {
            _sessionService = sessionService;
            InitializeComponent();
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