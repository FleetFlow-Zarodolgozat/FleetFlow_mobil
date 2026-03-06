using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using mobil.Models;
using mobil.Popups;
using mobil.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace mobil.ViewModels
{
    public partial class ServiceViewModel : ObservableObject
    {
        private readonly ServiceService _serviceService;
        private const int PageSize = 10;

        public ServiceViewModel(ServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        [ObservableProperty]
        ObservableCollection<Service> services = [];

        [ObservableProperty]
        bool isBusy;

        [ObservableProperty]
        bool isNewService;

        [ObservableProperty]
        string errorMessage = string.Empty;

        [ObservableProperty]
        bool hasLoaded;

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
        ServiceCreate newService = new()
        {
            Title = string.Empty,
            Description = string.Empty
        };

        public bool IsEmpty => HasLoaded && (Services == null || Services.Count == 0);
        public bool CanGoBack => CurrentPage > 1;
        public bool CanGoForward => CurrentPage < TotalPages;
        public string PageLabel => $"{CurrentPage} / {TotalPages}";

        public async Task LoadServices()
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
                var result = await _serviceService.MyServices(CurrentPage, PageSize);
                if (result is not null)
                {
                    Services = new ObservableCollection<Service>(result.Items);
                    TotalPages = Math.Max(result.TotalPages, 1);
                    TotalCount = result.TotalCount;
                }
                else
                {
                    Services = [];
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
            IsNewService = !IsNewService;
            HasError = false;
            HasSuccess = false;
        }

        [RelayCommand]
        async Task DeleteService(Service service)
        {
            bool confirm = await Application.Current!.Windows[0].Page!.DisplayAlert("Delete Service Request", $"Delete service?", "Delete", "Cancel");
            if (!confirm) return;
            try
            {
                IsBusy = true;
                HasError = false;
                HasSuccess = false;
                var error = await _serviceService.DeleteService(service.Id);
                if (error != null)
                {
                    HasError = true;
                    ErrorMessage = error;
                    return;
                }
                await FetchPage();
                HasSuccess = true;
                SuccessMessage = "Service request deleted successfully!";
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
        async Task CreateService()
        {
            try
            {
                IsBusy = true;
                HasError = false;
                HasSuccess = false;
                var error = await _serviceService.CreateService(NewService);
                if (error != null)
                {
                    HasError = true;
                    ErrorMessage = error;
                    return;
                }
                NewService = new ServiceCreate { Title = string.Empty, Description = string.Empty };
                CurrentPage = 1;
                await FetchPage();
                IsNewService = false;
                HasSuccess = true;
                SuccessMessage = "Service request created successfully!";
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
        async Task EditServiceDetails(Service service)
        {
            var detailsVm = IPlatformApplication.Current!.Services.GetRequiredService<ServiceDetailsViewModel>();
            detailsVm.Load(service);
            var popup = new ServiceDetailsPopup(detailsVm);
            await Shell.Current.ShowPopupAsync(popup);
            await FetchPage();
        }
    }
}
