using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using mobil.Models;
using mobil.Services;
using System.Collections.ObjectModel;

namespace mobil.ViewModels
{
    public partial class NotificationViewModel : ObservableObject
    {
        private readonly NotificationService _notificationService;
        public NotificationViewModel(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [ObservableProperty]
        ObservableCollection<Notification> notifications = new();

        [ObservableProperty]
        bool isBusy;

        public async Task Load()
        {
            await RefreshNots();
        }

        async Task RefreshNots()
        {
            IsBusy = true;
            try
            {
                var nots = await _notificationService.GetMine();
                Notifications.Clear();
                if (nots is not null)
                {
                    foreach (var e in nots)
                        Notifications.Add(e);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task MarkAsRead()
        {
            await _notificationService.MarkAsAllRead();
            await RefreshNots();
        }

        [RelayCommand]
        async Task Delete(ulong id)
        {
            await _notificationService.Delete(id);
            await RefreshNots();
        }
    }
}
