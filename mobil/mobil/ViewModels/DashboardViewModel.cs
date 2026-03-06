using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using mobil.Models;
using mobil.Popups;
using mobil.Services;
using Plugin.Maui.Calendar.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace mobil.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly DashboardService _dashboardService;
        public DashboardViewModel(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public ObservableCollection<Calendarevent> Events { get; set; } = new();

        [ObservableProperty]
        EventCollection calendarEvents = new();

        [ObservableProperty]
        Driver driver;

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
                var unreadStatus = await _dashboardService.HaveUnreadMessage();
                if (unreadStatus.HasValue)
                    HaveUnreadMessage = unreadStatus.Value;
                WelcomeMessage = Driver is not null ? $"Welcome back, {Driver.FullName}!" : "Welcome back!";
                LoadProfileImage();
                await RefreshCalendarEvents();
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

        async void LoadProfileImage()
        {
            if (Driver?.Id is null)
            {
                ProfileImage = new FontImageSource
                {
                    FontFamily = "FontAwesomeSolid",
                    Glyph = "\uf007",
                    Color = Colors.White,
                    Size = 16
                };
                return;
            }
            var image = await _dashboardService.GetDriverThumbnail(Driver.Id.Value);
            ProfileImage = image ?? new FontImageSource
            {
                FontFamily = "FontAwesomeSolid",
                Glyph = "\uf007",
                Color = Colors.White,
                Size = 16
            };
        }

        [RelayCommand]
        async Task EditProfile()
        {
            await Shell.Current.GoToAsync("//ProfilePage", new Dictionary<string, object>
            {
                { "IsEditing", true }
            });
        }

        [RelayCommand]
        async Task GoToNotifications()
        {
            await Shell.Current.GoToAsync("//NotificationPage");
        }

        [RelayCommand]
        async Task NewTrip()
        {
            await Shell.Current.GoToAsync("//TripPage", new Dictionary<string, object>
            {
                { "IsNewTrip", true }
            });
        }

        [RelayCommand]
        async Task NewFuelLog()
        {
            await Shell.Current.GoToAsync("//FuelPage", new Dictionary<string, object>
            {
                { "IsNewFuel", true }
            });
        }

        [RelayCommand]
        async Task DayTapped(DateTime date)
        {
            var dayEvents = Events.Where(e => e.StartAt.ToLocalTime().Date == date.Date).ToList();
            var popupVm = new CalendarDayViewModel(date, dayEvents, _dashboardService);
            await Shell.Current.ShowPopupAsync(new CalendarDayPopup(popupVm));
            await RefreshCalendarEvents();
        }

        async Task RefreshCalendarEvents()
        {
            var apiEvents = await _dashboardService.MyCalEvent();
            Events.Clear();
            var newCalendarEvents = new EventCollection();
            if (apiEvents is not null)
            {
                foreach (var e in apiEvents)
                    Events.Add(e);
                var grouped = apiEvents.GroupBy(e => e.StartAt.ToLocalTime().Date);
                foreach (var group in grouped)
                    newCalendarEvents.Add(group.Key, group.ToList());
            }
            CalendarEvents = newCalendarEvents;
        }
    }
}
