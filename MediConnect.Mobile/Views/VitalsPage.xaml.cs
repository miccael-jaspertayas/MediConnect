<<<<<<< HEAD
using System;
using Microsoft.Maui.Controls;
using MediConnect.Mobile.Services;
=======
using MediConnect.Mobile.Models;
using MediConnect.Mobile.ViewModels;
>>>>>>> main

namespace MediConnect.Mobile.Views;

public partial class VitalsPage : ContentPage
{
    public VitalsPage(VitalsViewModel vm)
    {
<<<<<<< HEAD
        private readonly SessionService _sessionService;

        // Constructor Injection (assuming registered in MauiProgram.cs)
        public VitalsPage(SessionService sessionService)
        {
            InitializeComponent();
            _sessionService = sessionService;
=======
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is VitalsViewModel vm)
        {
            await vm.LoadVitalsAsync();
>>>>>>> main
        }
    }

    // Open Add Vitals page
    private async void AddVitalsFab_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AddVitalsPage));
    }

    // Open an existing vital record
    private async void VitalCard_Tapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is VitalsModel vital)
        {
<<<<<<< HEAD
            // 1. Validate Heart Rate (Sane range: 30 to 250 bpm)
            if (!int.TryParse(HeartRateEntry.Text, out int heartRate) || heartRate < 30 || heartRate > 250)
            {
                await DisplayAlert("Validation Error", "Please enter a valid heart rate (30 - 250 bpm).", "OK");
                return;
            }

            // 2. Validate Temperature (Sane range: 34.0°C to 43.0°C)
            if (!double.TryParse(TemperatureEntry.Text, out double temperature) || temperature < 34.0 || temperature > 43.0)
            {
                await DisplayAlert("Validation Error", "Please enter a valid temperature (34.0°C - 43.0°C).", "OK");
                return;
            }

            // 3. Enforce Timestamp (Required - automatically capture current time)
            DateTime recordTimestamp = DateTime.Now;

            // Prepare the model
            var vitalData = new VitalModel
            {
                PatientId = _sessionService.PatientID,
                HeartRate = heartRate,
                Temperature = temperature,
                Timestamp = recordTimestamp
            };

            // TODO: Execute your API call (POST or PUT depending on if editing)
            // bool apiSuccess = await _apiService.SaveVitalAsync(vitalData);
            bool apiSuccess = true; // Simulated success for testing

            if (apiSuccess)
            {
                // Feed the saved vital entry back to the SessionService singleton
                _sessionService.UpdateMostRecentVital(vitalData);

                await DisplayAlert("Saved", "Vitals updated successfully!", "OK");
                await Shell.Current.GoToAsync("..");
            }
=======
            await Shell.Current.GoToAsync(
                $"{nameof(AddVitalsPage)}?vitalId={vital.VitalID}");
>>>>>>> main
        }
    }
}