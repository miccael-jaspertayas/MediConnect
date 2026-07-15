using MediConnect.Mobile.ViewModels;

namespace MediConnect.Mobile.Views
{
    [QueryProperty(nameof(VitalId), "vitalId")]
    public partial class AddVitalsPage : ContentPage
    {
        private readonly AddVitalsViewModel _vm;

        public AddVitalsPage(AddVitalsViewModel vm)
        {
            InitializeComponent();
            BindingContext = _vm = vm;
        }

        public int VitalId
        {
            set
            {
                if (value > 0)
                {
                    _ = _vm.LoadVitalAsync(value);
                }
            }
        }

        private async void BackButton_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}