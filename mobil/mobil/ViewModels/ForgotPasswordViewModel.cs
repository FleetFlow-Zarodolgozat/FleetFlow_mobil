using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        string email;

        [ObservableProperty]
        string errorMessage;

        [ObservableProperty]
        private bool isBusy;

        public Func<Task>? CloseAction;

        [RelayCommand]
        async Task Send()
        {
            ErrorMessage = "";
            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Email is required";
                return;
            }
            IsBusy = true;
            await _auth.ForgotPassword(new ForgotPassword { Email = Email });
            if (CloseAction != null) await CloseAction();
        }

        [RelayCommand]
        async Task Close()
        {
            if (CloseAction != null) await CloseAction();
        }
    }
}
