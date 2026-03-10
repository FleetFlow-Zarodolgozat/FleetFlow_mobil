using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using mobil.Models;
using mobil.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace mobil.ViewModels
{
    public partial class ServiceDetailsViewModel : ObservableObject
    {
        private readonly ServiceService _serviceService;
        private FileResult? _selectedPhoto;

        public ServiceDetailsViewModel(ServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        [ObservableProperty]
        Service service;

        [ObservableProperty]
        decimal driverReportCost;

        [ObservableProperty]
        string driverCloseNote = string.Empty;

        [ObservableProperty]
        bool isBusy;

        [ObservableProperty]
        string errorMessage = string.Empty;

        [ObservableProperty]
        bool hasError;

        [ObservableProperty]
        bool hasSuccess;

        [ObservableProperty]
        string successMessage = string.Empty;

        [ObservableProperty]
        bool hasNewPhoto;

        [ObservableProperty]
        ImageSource? previewImage;

        [ObservableProperty]
        bool isEdit;

        [ObservableProperty]
        string popupTitle = "Add Service Details";

        public Func<Task>? CloseAction { get; set; }

        public async void Load(Service svc)
        {
            service = svc;
            OnPropertyChanged(nameof(Service));
            isEdit = svc.DriverReportCost is not null && svc.DriverReportCost > 0;
            OnPropertyChanged(nameof(IsEdit));
            if (isEdit)
            {
                popupTitle = "Edit Service Details";
                driverReportCost = svc.DriverReportCost ?? 0;
            }
            else
            {
                popupTitle = "Add Service Details";
                driverReportCost = 0;
                driverCloseNote = string.Empty;
            }
            OnPropertyChanged(nameof(PopupTitle));
            OnPropertyChanged(nameof(DriverReportCost));
            OnPropertyChanged(nameof(DriverCloseNote));
            hasNewPhoto = false;
            previewImage = null;
            _selectedPhoto = null;
            hasError = false;
            hasSuccess = false;
            if (svc.InvoiceFileId is not null)
            {
                var file = await _serviceService.GetInvoiceFile(svc.InvoiceFileId!.Value);
                if (file.Stream != null)
                {
                    hasNewPhoto = true;
                    var image = ImageSource.FromStream(() => file.Stream);
                    previewImage = image;
                }
            }
            OnPropertyChanged(nameof(HasNewPhoto));
            OnPropertyChanged(nameof(PreviewImage));
            OnPropertyChanged(nameof(HasError));
            OnPropertyChanged(nameof(HasSuccess));
        }

        [RelayCommand]
        async Task PickPhoto()
        {
            try
            {
                var result = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Select invoice photo"
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
                    Title = "Take invoice photo"
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
        async Task Close()
        {
            if (CloseAction is not null)
                await CloseAction();
        }

        [RelayCommand]
        async Task Save()
        {
            if (driverReportCost <= 0)
            {
                HasError = true;
                ErrorMessage = "Please enter a valid cost amount.";
                return;
            }
            try
            {
                IsBusy = true;
                HasError = false;
                HasSuccess = false;
                var upload = new ServiceDetailUpload
                {
                    DriverReportCost = driverReportCost,
                    DriverCloseNote = driverCloseNote,
                    File = _selectedPhoto
                };
                string? error;
                if (isEdit)
                    error = await _serviceService.EditUploadedDetails(service.Id, upload);
                else
                    error = await _serviceService.UploadServiceDetails(service.Id, upload);
                if (error is not null)
                {
                    HasError = true;
                    ErrorMessage = error;
                    return;
                }
                HasSuccess = true;
                SuccessMessage = isEdit ? "Details updated!" : "Details uploaded!";
                await Task.Delay(1000);
                if (CloseAction is not null)
                    await CloseAction();
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
