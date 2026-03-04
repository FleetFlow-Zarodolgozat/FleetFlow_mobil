using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using mobil.Models;
using mobil.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace mobil.ViewModels
{
    public partial class ProfileViewModel : ObservableObject
    {
        private readonly DashboardService _dashboardService;
        private readonly ProfileService _profileService;
        private readonly SessionService _sessionService;
        public ProfileViewModel(DashboardService dashboardService, ProfileService profileService, SessionService sessionService)
        {
            _dashboardService = dashboardService;
            _profileService = profileService;
            _sessionService = sessionService;
        }

        [ObservableProperty]
        Driver? driver;

        [ObservableProperty]
        bool isLoading;

        [ObservableProperty]
        bool hasError;

        [ObservableProperty]
        string errorMessage = string.Empty;

        [ObservableProperty]
        bool hasSuccess;

        [ObservableProperty]
        string successMessage = string.Empty;

        [ObservableProperty]
        EditProfileData editProfileData = new();

        [ObservableProperty]
        ImageSource? profileImage;

        [ObservableProperty]
        bool isEditing;

        public async Task LoadData()
        {
            try
            {
                IsLoading = true;
                HasError = false;
                HasSuccess = false;
                await RefreshProfile();
                await LoadProfileImage();
                PopulateEditData();
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Failed to load profile data: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        async Task LoadProfileImage()
        {
            var fallback = new FontImageSource
            {
                FontFamily = "FontAwesomeSolid",
                Glyph = "\uf007",
                Color = Colors.White,
                Size = 60
            };
            if (Driver?.Id is null)
            {
                ProfileImage = fallback;
                return;
            }
            var image = await _dashboardService.GetDriverThumbnail(Driver.Id.Value);
            ProfileImage = image ?? fallback;
        }

        async Task RefreshProfile()
        {
            Driver = await _dashboardService.MyProfileData();
        }

        void PopulateEditData()
        {
            if (Driver == null) return;
            EditProfileData = new EditProfileData
            {
                FullName = Driver.FullName,
                Phone = Driver.Phone,
            };
        }

        [RelayCommand]
        void ToggleEdit()
        {
            IsEditing = !IsEditing;
            if (IsEditing)
            {
                PopulateEditData();
            }
            HasError = false;
            HasSuccess = false;
        }

        [RelayCommand]
        async Task LogOut()
        {
            _sessionService.Logout();
            await Shell.Current.GoToAsync("//LoginPage");
        }

        [RelayCommand]
        async Task SaveChanges()
        {
            try
            {
                IsLoading = true;
                HasError = false;
                HasSuccess = false;
                var data = new EditProfileData
                {
                    FullName = EditProfileData.FullName,
                    Phone = EditProfileData.Phone,
                    Password = EditProfileData.Password,
                    PasswordAgain = EditProfileData.PasswordAgain,
                };
                var error = await _profileService.UpdateMyData(data);
                if (error != null)
                {
                    HasError = true;
                    ErrorMessage = error;
                    return;
                }
                await RefreshProfile();
                PopulateEditData();
                IsEditing = false;
                HasSuccess = true;
                SuccessMessage = "Profile updated successfully!";
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Failed to save: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
