using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using mobil.Models;
using mobil.Services;
using System.Collections.ObjectModel;

namespace mobil.ViewModels
{
    public partial class CalendarDayViewModel : ObservableObject
    {
        public DateTime SelectedDate { get; }
        public ObservableCollection<Calendarevent> DayEvents { get; } = [];
        public bool HasChanges { get; private set; }

        [ObservableProperty]
        string? titleInput;

        [ObservableProperty]
        string? descriptionInput;

        [ObservableProperty]
        TimeSpan startTime = new(9, 0, 0);

        [ObservableProperty]
        TimeSpan endTime = new(10, 0, 0);

        [ObservableProperty]
        bool isCreating;

        [ObservableProperty]
        bool showForm;

        [ObservableProperty]
        string? formError;

        public Func<Task>? CloseAction { get; set; }

        private readonly DashboardService _service;

        public CalendarDayViewModel(DateTime date, List<Calendarevent> events, DashboardService service)
        {
            SelectedDate = date;
            _service = service;
            foreach (var e in events)
                DayEvents.Add(e);
        }

        [RelayCommand]
        void ToggleForm()
        {
            ShowForm = !ShowForm;
            if (!ShowForm)
            {
                TitleInput = null;
                DescriptionInput = null;
                FormError = null;
            }
        }

        [RelayCommand]
        async Task CreateEvent()
        {
            if (string.IsNullOrWhiteSpace(TitleInput))
            {
                FormError = "Title is required.";
                return;
            }
            FormError = null;
            IsCreating = true;
            var newEvent = new Calendarevent
            {
                Title = TitleInput,
                Description = DescriptionInput,
                StartAt = SelectedDate.Date + StartTime,
                EndAt = SelectedDate.Date + EndTime
            };
            var error = await _service.CreateEvent(newEvent);
            IsCreating = false;
            if (error != null)
            {
                FormError = error;
                return;
            }
            DayEvents.Add(newEvent);
            HasChanges = true;
            TitleInput = null;
            DescriptionInput = null;
            ShowForm = false;
        }

        [RelayCommand]
        async Task DeleteEvent(Calendarevent ev)
        {
            bool confirm = await Application.Current!.Windows[0].Page!.DisplayAlert("Delete Event", $"Delete '{ev.Title}'?", "Delete", "Cancel");
            if (!confirm) return;
            var error = await _service.DeleteEvent(ev.Id);
            if (error != null)
            {
                FormError = error;
                return;
            }
            DayEvents.Remove(ev);
            HasChanges = true;
        }

        [RelayCommand]
        async Task Close()
        {
            if (CloseAction is not null)
                await CloseAction();
        }
    }
}
