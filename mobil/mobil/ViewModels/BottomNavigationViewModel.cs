using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace mobil.ViewModels
{
    public partial class BottomNavigationViewModel : ObservableObject
    {
        [RelayCommand]
        async Task GoToDashboard()
        {
            await Shell.Current.GoToAsync("//DashboardPage");
        }

        [RelayCommand]
        async Task GoToTripPage()
        {
            await Shell.Current.GoToAsync("//TripPage");
        }
    }
}
