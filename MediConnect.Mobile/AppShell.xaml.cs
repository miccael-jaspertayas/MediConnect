using MediConnect.Mobile.Views;

namespace MediConnect.Mobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("Profile", typeof(Views.ProfilePage));
            Routing.RegisterRoute(nameof(AddVitalsPage), typeof(AddVitalsPage));
        }
    }
}
