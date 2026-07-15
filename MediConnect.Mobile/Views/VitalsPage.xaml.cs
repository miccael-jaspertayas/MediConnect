using MediConnect.Mobile.Models;
using MediConnect.Mobile.ViewModels;

namespace MediConnect.Mobile.Views;

public partial class VitalsPage : ContentPage
{
    public VitalsPage(VitalsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        
        if (BindingContext is VitalsViewModel viewModel)
        {
            _ = viewModel.LoadVitalsAsync();
        }
    }

    // Navigate to Add Vitals page (New)
    private async void AddVitalsFab_Clicked(object sender, EventArgs e)
    {
        // Navigates to create a fresh record
        await Shell.Current.GoToAsync(nameof(AddVitalsPage));
    }

    // Open an existing vital for editing
    private async void VitalCard_Tapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is VitalsModel vital)
        {
            // Navigates with the ID to put the page in Edit Mode
            await Shell.Current.GoToAsync(
                $"{nameof(AddVitalsPage)}?VitalId={vital.VitalID}");
        }
    }
}