using MediConnect.Mobile.Models;
using MediConnect.Mobile.ViewModels;

namespace MediConnect.Mobile.Views
{
    public partial class VitalsPage : ContentPage
    {
        private readonly VitalsViewModel _viewModel;

        public VitalsPage(VitalsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadVitalsAsync();
        }

        private async void VitalCard_Tapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is VitalsModel vital)
            {
                // Present a native pop-up option without altering the UI layout
                string action = await DisplayActionSheetAsync(
                    "Vitals Options",
                    "Cancel",
                    null,
                    "Edit Record",
                    "Delete Record");

                if (action == "Edit Record")
                {
                    // Navigate to Edit screen
                    await Shell.Current.GoToAsync($"{nameof(AddVitalsPage)}?Id={vital.VitalID}");
                }
                else if (action == "Delete Record")
                {
                    // Confirm and delete
                    bool confirm = await DisplayAlertAsync(
                        "Confirm Delete",
                        "Are you sure you want to delete this vital record?",
                        "Yes",
                        "No");

                    if (confirm)
                    {
                        await _viewModel.DeleteVitalAsync(vital);
                    }
                }
            }
        }

        private async void AddVitalsFab_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(AddVitalsPage));
        }
    }
}