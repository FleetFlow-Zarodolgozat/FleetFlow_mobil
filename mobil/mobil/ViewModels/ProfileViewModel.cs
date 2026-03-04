using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using mobil.Models;
using mobil.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        string? editFullName;

        [ObservableProperty]
        string? editPhone;

        [ObservableProperty]
        string? editPassword;

        [ObservableProperty]
        string? editPasswordAgain;

        [ObservableProperty]
        ImageSource? profileImage;

        [ObservableProperty]
        ImageSource? editPreviewImage;

        [ObservableProperty]
        bool isEditing;

        [ObservableProperty]
        bool hasNewPhoto;

        [ObservableProperty]
        bool hasProfileImage;

        partial void OnDriverChanged(Driver? value)
        {
            HasProfileImage = value?.ProfileImgFileId.HasValue == true;
        }

        private FileResult? _selectedPhoto;

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
            EditFullName = Driver.FullName;
            EditPhone = Driver.Phone;
            EditPassword = null;
            EditPasswordAgain = null;
            _selectedPhoto = null;
            HasNewPhoto = false;
            EditPreviewImage = null;
        }

        [RelayCommand]
        void ToggleEdit()
        {
            IsEditing = !IsEditing;
            if (IsEditing)
                PopulateEditData();
            HasError = false;
            HasSuccess = false;
        }

        [RelayCommand]
        async Task PickPhoto()
        {
            try
            {
                var result = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Select profile photo"
                });
                if (result is not null)
                    await SetSelectedPhoto(result);
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Failed to pick photo: {ex.Message}";
            }
        }

        [RelayCommand]
        async Task TakePhoto()
        {
            if (!MediaPicker.Default.IsCaptureSupported)
            {
                HasError = true;
                ErrorMessage = "Camera is not available on this device.";
                return;
            }
            try
            {
                var result = await MediaPicker.Default.CapturePhotoAsync(new MediaPickerOptions
                {
                    Title = "Take profile photo"
                });
                if (result is not null)
                    await SetSelectedPhoto(result);
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Failed to capture photo: {ex.Message}";
                Debug.WriteLine(ex.Message);
            }
        }

        async Task SetSelectedPhoto(FileResult result)
        {
            _selectedPhoto = result;
            HasNewPhoto = true;
            var stream = await result.OpenReadAsync();
            EditPreviewImage = ImageSource.FromStream(() => stream);
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
                    FullName = EditFullName,
                    Phone = EditPhone,
                    Password = EditPassword,
                    PasswordAgain = EditPasswordAgain,
                    File = _selectedPhoto
                };
                var error = await _profileService.UpdateMyData(data);
                if (error != null)
                {
                    HasError = true;
                    ErrorMessage = error;
                    return;
                }
                await RefreshProfile();
                await LoadProfileImage();
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

        [RelayCommand]
        async Task DeleteProfileImage()
        {
            if (Driver?.ProfileImgFileId is null) return;
            var success = await _profileService.DeleteProfileImg(Driver.ProfileImgFileId.Value);
            if (success)
            {
                ProfileImage = new FontImageSource
                {
                    FontFamily = "FontAwesomeSolid",
                    Glyph = "\uf007",
                    Color = Colors.White,
                    Size = 60
                };
                await RefreshProfile();
                HasNewPhoto = false;
                EditPreviewImage = null;
            }
            else
            {
                HasError = true;
                ErrorMessage = "Failed to delete profile image.";
            }
        }
    }
}
