using Microsoft.Maui.Controls;
using System;

namespace MediConnect.Mobile.Views
{
    public partial class VitalsPage : ContentPage
    {
        public VitalsPage()
        {
            InitializeComponent();
        }

        private async void OnSaveVitalsClicked(object sender, EventArgs e)
        {
            // Simple validation check
            if (string.IsNullOrWhiteSpace(HeartRateEntry.Text) &&
                string.IsNullOrWhiteSpace(SystolicEntry.Text) &&
                string.IsNullOrWhiteSpace(TemperatureEntry.Text))
            {
                await DisplayAlert("Empty Fields", "Please enter at least one vital sign value to save.", "OK");
                return;
            }

            // TODO: Package these up and call your local API at port 5016!

            await DisplayAlert("Vitals Saved", "Your metrics have been successfully uploaded to the system.", "Great");

            // Go back or go to dashboard
            await Shell.Current.GoToAsync("..");
        }
    }
}