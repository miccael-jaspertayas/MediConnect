using MediConnect.Mobile.Views;

namespace MediConnect.Mobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register your new vitals page routing string here
            Routing.RegisterRoute(nameof(AddVitalsPage), typeof(AddVitalsPage));
        }
    }
}
