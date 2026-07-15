using MediConnect.Mobile.ViewModels;


namespace MediConnect.Mobile.Views
{
    public partial class DashboardPage : ContentPage
    {
        private readonly DashboardViewModel _vm;

        public DashboardPage(DashboardViewModel vm)
        {
            InitializeComponent();
            BindingContext = _vm = vm;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _vm.OnAppearing();

           
        }
    }
}

