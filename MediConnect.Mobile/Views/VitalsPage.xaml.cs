using System;
using Microsoft.Maui.Controls;
using MediConnect.Mobile.Models;
using MediConnect.Mobile.ViewModels;

namespace MediConnect.Mobile.Views
{
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

            // Safely trigger the ViewModel to load the list of vitals from the API
            if (BindingContext is VitalsViewModel vm)
            {
                await vm.LoadVitalsAsync();
            }
        }

        // Open the Add Vitals page
        private async void AddVitalsFab_Clicked(object? sender, EventArgs? e)
        {
            await Shell.Current.GoToAsync(nameof(AddVitalsPage));
        }

        // Open an existing vital record for editing
        private async void VitalCard_Tapped(object? sender, TappedEventArgs? e)
        {
            if (e?.Parameter is VitalsModel vital)
            {
                // Route to AddVitalsPage passing the VitalID as a query parameter
                await Shell.Current.GoToAsync($"{nameof(AddVitalsPage)}?Id={vital.VitalID}");
            }
        }
    }
}