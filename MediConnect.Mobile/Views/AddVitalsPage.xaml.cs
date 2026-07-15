using MediConnect.Mobile.ViewModels;

namespace MediConnect.Mobile.Views;

public partial class AddVitalsPage : ContentPage
{
    public AddVitalsPage(VitalsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    // Go back to the previous page
    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}