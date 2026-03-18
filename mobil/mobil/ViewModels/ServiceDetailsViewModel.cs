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
        Service service = new Service();

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

        public async Task Load(Service svc)
        {
            Service = svc;
            IsEdit = svc.DriverReportCost is not null && svc.DriverReportCost > 0;
            if (IsEdit)
            {
                PopupTitle = "Edit Service Details";
                DriverReportCost = svc.DriverReportCost ?? 0;
            }
            else
            {
                PopupTitle = "Add Service Details";
                DriverReportCost = 0;
                DriverCloseNote = string.Empty;
            }
            HasNewPhoto = false;
            PreviewImage = null;
            _selectedPhoto = null;
            HasError = false;
            HasSuccess = false;
            if (svc.InvoiceFileId is not null)
            {
                var file = await _serviceService.GetInvoiceFile(svc.InvoiceFileId!.Value);
                if (file.Stream != null)
                {
                    HasNewPhoto = true;
                    var image = ImageSource.FromStream(() => file.Stream);
                    PreviewImage = image;
                }
            }
        }

        [RelayCommand]
        async Task PickPhoto()
        {
            try
            {
                var results = await MediaPicker.Default.PickPhotosAsync(new MediaPickerOptions
                {
                    Title = "Select invoice photo"
                });
                if (results is not null && results.Count > 0)
                    await SetSelectedPhoto(results[0]);
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
            if (DriverReportCost <= 0)
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
                    DriverReportCost = DriverReportCost,
                    DriverCloseNote = DriverCloseNote,
                    File = _selectedPhoto
                };
                string? error;
                if (IsEdit)
                    error = await _serviceService.EditUploadedDetails(Service.Id, upload);
                else
                    error = await _serviceService.UploadServiceDetails(Service.Id, upload);
                if (error is not null)
                {
                    HasError = true;
                    ErrorMessage = error;
                    return;
                }
                HasSuccess = true;
                SuccessMessage = IsEdit ? "Details updated!" : "Details uploaded!";
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
