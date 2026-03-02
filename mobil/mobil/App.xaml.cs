using Microsoft.Extensions.DependencyInjection;
using mobil.Services;

namespace mobil
{
    public partial class App : Application
    {
        public App(SessionService sessionService)
        {
            InitializeComponent();
            CheckLogin(sessionService);
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        private async void CheckLogin(SessionService session)
        {
            var token = await session.GetToken();
            if (!string.IsNullOrEmpty(token))
                await Shell.Current.GoToAsync("//DashboardPage");
            else
                await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}