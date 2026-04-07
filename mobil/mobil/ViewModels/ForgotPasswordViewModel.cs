using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using mobil.Models;
using mobil.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace mobil.ViewModels
{
    public partial class ForgotPasswordViewModel : ObservableObject
    {
        private readonly AuthService _auth;

        public ForgotPasswordViewModel(AuthService auth)
        {
            _auth = auth;
        }

        [ObservableProperty]
        string email = string.Empty;

        [ObservableProperty]
        string errorMessage = string.Empty;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private bool showOpenEmailButton;

        public Func<Task>? CloseAction;

        [RelayCommand]
        async Task Send()
        {
            ErrorMessage = "";
            ShowOpenEmailButton = false;
            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Email is required";
                return;
            }
            try
            {
                IsBusy = true;
                await _auth.ForgotPassword(new ForgotPassword { Email = Email });
                ShowOpenEmailButton = true;
            }
            catch
            {
                ErrorMessage = "Failed to send reset email. Please try again.";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task OpenEmailApp()
        {
            try
            {
                await Launcher.Default.OpenAsync(new Uri("mailto:"));
            }
            catch
            {
                ErrorMessage = "Could not open email application.";
            }
        }

        [RelayCommand]
        async Task Close()
        {
            Email = "";
            ErrorMessage = "";
            ShowOpenEmailButton = false;
            if (CloseAction != null)
                await CloseAction();
        }
    }
}
