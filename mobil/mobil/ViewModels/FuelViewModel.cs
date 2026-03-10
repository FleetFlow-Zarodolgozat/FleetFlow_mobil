using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using mobil.Models;
using mobil.Services;
using System.Collections.ObjectModel;

namespace mobil.ViewModels
{
    public partial class FuelViewModel : ObservableObject, IQueryAttributable
    {
        private readonly FuelService _fuelService;
        private const int PageSize = 10;
        private FileResult? _selectedPhoto;

        public FuelViewModel(FuelService fuelService)
        {
            _fuelService = fuelService;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("IsNewFuel"))
                IsNewFuel = (bool)query["IsNewFuel"];
        }

        [ObservableProperty]
        ObservableCollection<Fuel> fuels = [];

        [ObservableProperty]
        bool isBusy;

        [ObservableProperty]
        bool isNewFuel;

        [ObservableProperty]
        string errorMessage = string.Empty;

        [ObservableProperty]
        bool hasLoaded;

        [ObservableProperty]
        FuelCreate newFuel = new()
        {
            Date = DateTime.Now,
            OdometerKm = 0,
            Liters = 0,
            TotalCost = 0
        };

        [ObservableProperty]
        DateTime newFuelDate = DateTime.Now;

        [ObservableProperty]
        TimeSpan newFuelTime = DateTime.Now.TimeOfDay;

        partial void OnNewFuelDateChanged(DateTime value) =>
            NewFuel.Date = value.Date + NewFuelTime;

        partial void OnNewFuelTimeChanged(TimeSpan value) =>
            NewFuel.Date = NewFuelDate.Date + value;

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

        [ObservableProperty]
        ImageSource? previewImage;

        [ObservableProperty]
        bool hasNewPhoto;

        public bool IsEmpty => HasLoaded && (Fuels == null || Fuels.Count == 0);
        public bool CanGoBack => CurrentPage > 1;
        public bool CanGoForward => CurrentPage < TotalPages;
        public string PageLabel => $"{CurrentPage} / {TotalPages}";

        public async Task LoadFuels()
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
                var result = await _fuelService.MyFuels(CurrentPage, PageSize);
                if (result is not null)
                {
                    Fuels = new ObservableCollection<Fuel>(result.Items);
                    TotalPages = Math.Max(result.TotalPages, 1);
                    TotalCount = result.TotalCount;
                }
                else
                {
                    Fuels = [];
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
        void ToggleNew()
        {
            IsNewFuel = !IsNewFuel;
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
                    Title = "Select fuel receipt"
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
                    Title = "Take fuel receipt photo"
                });
                if (result is not null)
                    await SetSelectedPhoto(result);
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Failed to capture photo: {ex.Message}";
            }
        }

        async Task SetSelectedPhoto(FileResult result)
        {
            _selectedPhoto = result;
            HasNewPhoto = true;
            var stream = await result.OpenReadAsync();
            PreviewImage = ImageSource.FromStream(() => stream);
        }

        [RelayCommand]
        async Task DeleteFuel(Fuel fuel)
        {
            bool confirm = await Application.Current!.Windows[0].Page!.DisplayAlert("Delete Fuel Log", $"Delete fuel entry from {fuel.Date:yyyy.MM.dd}?", "Delete", "Cancel");
            if (!confirm) return;
            try
            {
                IsBusy = true;
                HasError = false;
                HasSuccess = false;
                var error = await _fuelService.DeleteFuel(fuel.Id);
                if (error != null)
                {
                    HasError = true;
                    ErrorMessage = error;
                    return;
                }
                await FetchPage();
                HasSuccess = true;
                SuccessMessage = "Fuel log deleted successfully!";
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
        async Task CreateFuel()
        {
            try
            {
                IsBusy = true;
                HasError = false;
                HasSuccess = false;
                NewFuel.File = _selectedPhoto;
                var error = await _fuelService.CreateFuel(NewFuel);
                if (error != null)
                {
                    HasError = true;
                    ErrorMessage = error;
                    return;
                }
                NewFuel = new FuelCreate { Date = DateTime.Now, OdometerKm = 0, Liters = 0, TotalCost = 0 };
                NewFuelDate = DateTime.Now;
                NewFuelTime = DateTime.Now.TimeOfDay;
                _selectedPhoto = null;
                PreviewImage = null;
                HasNewPhoto = false;
                CurrentPage = 1;
                await FetchPage();
                IsNewFuel = false;
                HasSuccess = true;
                SuccessMessage = "Fuel log created successfully!";
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
