using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using mobil.Models;
using mobil.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace mobil.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly AuthService _authService;
        private readonly SessionService _session;

        public LoginViewModel(AuthService authService, SessionService session)
        {
            _authService = authService;
            _session = session;
        }

        [ObservableProperty]
        string? emailInput;

        [ObservableProperty]
        string? pwdInput;

        [RelayCommand]
        async Task Login()
        {
            var loginData = new Login
            {
                Email = EmailInput,
                Password = PwdInput
            };
            var token = await _authService.Login(loginData);
            if (token != null)
            {
                await _session.SaveToken(token);
                await Shell.Current.GoToAsync("//DashboardPage");
            }
            else
                // Handle login failure (e.g., show an error message from the server)
                await Shell.Current.DisplayAlert("Login Failed", "Invalid email or password.", "OK");
        }
    }
}
