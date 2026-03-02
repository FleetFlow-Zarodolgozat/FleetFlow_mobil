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

        public LoginViewModel(AuthService authService)
        {
            _authService = authService;
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
                await SecureStorage.SetAsync("bearer_token", token);
                await Shell.Current.DisplayAlert("Siker", "Bejelentkezve", "OK");
            }
            else
                await Shell.Current.DisplayAlert("Hiba", "Sikertelen login", "OK");
        }
    }
}
