using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using mobil.Handlers;
using mobil.Pages;
using mobil.Pages.Components;
using mobil.Popups;
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
                    fonts.AddFont("fa-solid-900.ttf", "FontAwesomeSolid");
                    fonts.AddFont("fa-regular-400.ttf", "FontAwesomeRegular");
                });
            builder.Services.AddHttpClient<AuthService>(client =>
            {
                client.BaseAddress = new Uri("http://fleetflow-zarodolgozat-backend-ressdominik.jcloud.jedlik.cloud/api/");
            }).AddHttpMessageHandler<AuthHttpHandler>();
            builder.Services.AddHttpClient<DashboardService>(client =>
            {
                client.BaseAddress = new Uri("http://fleetflow-zarodolgozat-backend-ressdominik.jcloud.jedlik.cloud/api/");
            }).AddHttpMessageHandler<AuthHttpHandler>();
            builder.Services.AddHttpClient<NotificationService>(client =>
            {
                client.BaseAddress = new Uri("http://fleetflow-zarodolgozat-backend-ressdominik.jcloud.jedlik.cloud/api/");
            }).AddHttpMessageHandler<AuthHttpHandler>();
            builder.Services.AddHttpClient<ProfileService>(client =>
            {
                client.BaseAddress = new Uri("http://fleetflow-zarodolgozat-backend-ressdominik.jcloud.jedlik.cloud/api/");
            }).AddHttpMessageHandler<AuthHttpHandler>();
            builder.Services.AddHttpClient<TripService>(client =>
            {
                client.BaseAddress = new Uri("http://fleetflow-zarodolgozat-backend-ressdominik.jcloud.jedlik.cloud/api/");
            }).AddHttpMessageHandler<AuthHttpHandler>();
            builder.Services.AddTransient<AuthHttpHandler>();
            builder.Services.AddSingleton<SessionService>();
            builder.Services.AddSingleton<BottomNavigationViewModel>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<ForgotPasswordPopup>();
            builder.Services.AddTransient<ForgotPasswordViewModel>();
            builder.Services.AddTransient<DashboardPage>();
            builder.Services.AddTransient<DashboardViewModel>();
            builder.Services.AddTransient<NotificationPage>();
            builder.Services.AddTransient<NotificationViewModel>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<ProfileViewModel>();
            builder.Services.AddTransient<TripPage>();
            builder.Services.AddTransient<TripViewModel>();
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
