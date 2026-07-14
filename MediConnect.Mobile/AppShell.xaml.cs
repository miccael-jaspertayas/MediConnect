using MediConnect.Mobile.Views;

namespace MediConnect.Mobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
            Routing.RegisterRoute(nameof(AddRecordPage), typeof(AddRecordPage));
        }
    }
}
