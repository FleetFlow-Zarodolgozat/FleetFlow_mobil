using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using mobil.Models;
using mobil.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace mobil.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly DashboardService _dashboardService;
        public DashboardViewModel(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
            //LoadProfileImage();
        }

        [ObservableProperty]
        Driver driver;

        [ObservableProperty]
        List<Calendarevent> calendarEvents;

        [ObservableProperty]
        Vehicle vehicle;

        [ObservableProperty]
        Stats stats;

        [ObservableProperty]
        bool haveUnreadMessage;

        [ObservableProperty]
        bool isBusy;

        [ObservableProperty]
        string errorMessage;

        [ObservableProperty]
        string welcomeMessage;

        [ObservableProperty]
        ImageSource? profileImage;

        public async Task LoadData()
        {
            try
            {
                IsBusy = true;
                Driver = await _dashboardService.MyProfileData();
                Vehicle = await _dashboardService.MyVehicle();
                Stats = await _dashboardService.MyStats();
                CalendarEvents = await _dashboardService.MyCalEvent();
                var unreadStatus = await _dashboardService.HaveUnreadMessage();
                if (unreadStatus.HasValue)
                    HaveUnreadMessage = unreadStatus.Value;
                WelcomeMessage = $"Welcome back, {Driver.FullName}!";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load dashboard data: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        //async void LoadProfileImage()
        //{
        //    var image = await _dashboardService.GetDriverThumbnail(Driver.ProfileImgFileId);
        //    ProfileImage = image ?? ImageSource.FromFile("default_driver.png");
        //}

        [RelayCommand]
        async Task EditProfile()
        {
            // TODO: Navigate to profile edit page
        }

        [RelayCommand]
        async Task NewTrip()
        {
            // TODO: Navigate to new trip page
        }

        [RelayCommand]
        async Task NewFuelLog()
        {
            // TODO: Navigate to new fuel log page
        }
    }
}
