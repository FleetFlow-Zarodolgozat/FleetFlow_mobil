using mobil.Pages;

namespace mobil
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("NotificationPage", typeof(NotificationPage));
            Routing.RegisterRoute("ProfilePage", typeof(ProfilePage));
            Routing.RegisterRoute("FuelPage", typeof(FuelPage));
            Routing.RegisterRoute("TripPage", typeof(TripPage));
        }
    }
}
