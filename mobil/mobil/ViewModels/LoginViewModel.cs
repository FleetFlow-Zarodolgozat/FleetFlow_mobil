using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using mobil.Models;
using mobil.Popups;
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
        private readonly ForgotPasswordPopup _popup;
        public LoginViewModel(AuthService authService, SessionService session, ForgotPasswordPopup popup)
        {
            _authService = authService;
            _session = session;
            _popup = popup;
        }

        [ObservableProperty]
        private string? email;

        [ObservableProperty]
        private string? password;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private bool hasError;

        [ObservableProperty]
        private string? errorMessage;

        [ObservableProperty]
        private bool isPasswordVisible;

        [ObservableProperty]
        private bool passwordVisibilityIcon;

        [RelayCommand]
        void TogglePasswordVisibility()
        {
            IsPasswordVisible = !IsPasswordVisible;
            PasswordVisibilityIcon = IsPasswordVisible;
        }

        [RelayCommand]
        async Task ForgotPassword()
        {
            await Shell.Current.ShowPopupAsync(_popup);
        }

        [RelayCommand]
        async Task Login()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                HasError = true;
                ErrorMessage = "Please enter both email and password.";
                return;
            }
            try
            {
                IsBusy = true;
                HasError = false;
                ErrorMessage = null;
                var loginData = new Login
                {
                    Email = Email,
                    Password = Password
                };
                var token = await _authService.Login(loginData);
                if (token != null)
                {
                    await _session.SaveToken(token);
                    await Shell.Current.GoToAsync("//DashboardPage");
                }
                else
                {
                    HasError = true;
                    ErrorMessage = "Invalid email or password.";
                }
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = "An error occurred during login. Please try again.";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
