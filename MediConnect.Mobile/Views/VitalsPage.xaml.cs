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

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is VitalsViewModel vm)
        {
            await vm.LoadVitalsAsync();
        }
    }

    private async void AddVitalsFab_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AddVitalsPage));
    }

    private async void VitalCard_Tapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is Vitals vital)
        {
            await Shell.Current.GoToAsync(
                $"{nameof(AddVitalsPage)}?vitalId={vital.VitalID}");
        }
    }
}