using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using mobil.Handlers;
using mobil.Services;
using mobil.ViewModels;

namespace mobil
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            builder.Services.AddHttpClient<AuthService>(client =>
            {
                client.BaseAddress = new Uri("https://fleetflow-zarodolgozat-backend-ressdominik.jcloud.jedlik.cloud/api/");
            }).AddHttpMessageHandler<AuthHttpHandler>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<AuthHttpHandler>();
            builder.Services.AddSingleton<SessionService>();
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
