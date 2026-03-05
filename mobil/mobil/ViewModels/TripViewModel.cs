using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using mobil.Models;
using mobil.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace mobil.ViewModels
{
    public partial class TripViewModel : ObservableObject, IQueryAttributable
    {
        private readonly TripService _tripService;
        private const int PageSize = 10;

        public TripViewModel(TripService tripService)
        {
            _tripService = tripService;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("IsNewTrip"))
            {
                IsNewTrip = (bool)query["IsNewTrip"];
            }
        }

        [ObservableProperty]
        ObservableCollection<Trip> trips = [];

        [ObservableProperty]
        bool isBusy;

        [ObservableProperty]
        bool isNewTrip;

        [ObservableProperty]
        string errorMessage = string.Empty;

        [ObservableProperty]
        bool hasLoaded;

        [ObservableProperty]
        TripCreate newTrip = new()
        {
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddMinutes(30),
            DistanceKm = 0,
            StartOdometerKm = 0,
            EndOdometerKm = 0
        };

        [ObservableProperty]
        DateTime newStartDate = DateTime.Now;

        [ObservableProperty]
        TimeSpan newStartTime = DateTime.Now.TimeOfDay;

        [ObservableProperty]
        DateTime newEndDate = DateTime.Now;

        [ObservableProperty]
        TimeSpan newEndTime = DateTime.Now.AddMinutes(30).TimeOfDay;

        partial void OnNewStartDateChanged(DateTime value) =>
            NewTrip.StartTime = value.Date + NewStartTime;

        partial void OnNewStartTimeChanged(TimeSpan value) =>
            NewTrip.StartTime = NewStartDate.Date + value;

        partial void OnNewEndDateChanged(DateTime value) =>
            NewTrip.EndTime = value.Date + NewEndTime;

        partial void OnNewEndTimeChanged(TimeSpan value) =>
            NewTrip.EndTime = NewEndDate.Date + value;

        [ObservableProperty]
        bool hasSuccess;

        [ObservableProperty]
        string successMessage = string.Empty;

        [ObservableProperty]
        bool hasError;

        [ObservableProperty]
        int currentPage = 1;

        [ObservableProperty]
        int totalPages = 1;

        [ObservableProperty]
        int totalCount;

        public bool IsEmpty => HasLoaded && (Trips == null || Trips.Count == 0);
        public bool CanGoBack => CurrentPage > 1;
        public bool CanGoForward => CurrentPage < TotalPages;
        public string PageLabel => $"{CurrentPage} / {TotalPages}";

        public async Task LoadTrips()
        {
            CurrentPage = 1;
            await FetchPage();
        }

        async Task FetchPage()
        {
            try
            {
                IsBusy = true;
                HasError = false;
                HasSuccess = false;
                HasLoaded = false;
                var result = await _tripService.MyTrips(CurrentPage, PageSize);
                if (result is not null)
                {
                    Trips = new ObservableCollection<Trip>(result.Items);
                    TotalPages = Math.Max(result.TotalPages, 1);
                    TotalCount = result.TotalCount;
                }
                else
                {
                    Trips = [];
                    TotalPages = 1;
                    TotalCount = 0;
                }
                HasLoaded = true;
                NotifyPagination();
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        void NotifyPagination()
        {
            OnPropertyChanged(nameof(IsEmpty));
            OnPropertyChanged(nameof(CanGoBack));
            OnPropertyChanged(nameof(CanGoForward));
            OnPropertyChanged(nameof(PageLabel));
        }

        [RelayCommand]
        async Task PreviousPage()
        {
            if (!CanGoBack) return;
            CurrentPage--;
            await FetchPage();
        }

        [RelayCommand]
        async Task NextPage()
        {
            if (!CanGoForward) return;
            CurrentPage++;
            await FetchPage();
        }

        [RelayCommand]
        async Task CreateTrip()
        {
            try
            {
                IsBusy = true;
                HasError = false;
                HasSuccess = false;
                var error = await _tripService.CreateTrip(NewTrip);
                if (error != null)
                {
                    HasError = true;
                    ErrorMessage = error;
                    return;
                }
                NewTrip = new TripCreate
                {
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddMinutes(30),
                    DistanceKm = 0,
                    StartOdometerKm = 0,
                    EndOdometerKm = 0
                };
                NewStartDate = DateTime.Now;
                NewStartTime = DateTime.Now.TimeOfDay;
                NewEndDate = DateTime.Now;
                NewEndTime = DateTime.Now.AddMinutes(30).TimeOfDay;
                CurrentPage = 1;
                await FetchPage();
                IsNewTrip = false;
                HasSuccess = true;
                SuccessMessage = "Trip created successfully!";
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        void ToggleNew()
        {
            IsNewTrip = !IsNewTrip;
            HasError = false;
            HasSuccess = false;
        }

        [RelayCommand]
        async Task DeleteTrip(Trip trip)
        {
            bool confirm = await Application.Current!.Windows[0].Page!.DisplayAlert("Delete Trip", $"Delete trip from {trip.StartLocation} to {trip.EndLocation}?", "Delete", "Cancel");
            if (!confirm) return;
            try
            {
                IsBusy = true;
                HasError = false;
                HasSuccess = false;
                var error = await _tripService.DeleteTrip(trip.Id);
                if (error != null)
                {
                    HasError = true;
                    ErrorMessage = error;
                    return;
                }
                await FetchPage();
                HasSuccess = true;
                SuccessMessage = "Trip deleted successfully!";
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
