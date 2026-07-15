using MediConnect.Mobile.ViewModels;

namespace MediConnect.Mobile.Views;

public partial class AddVitalsPage : ContentPage
{
    public AddVitalsPage(VitalsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

<<<<<<< HEAD
        public AddVitalsPage(VitalsService vitalsService)
        {
            InitializeComponent();
            _vitalsService = vitalsService;
        }

        private async void OnSaveVitalsClicked(object sender, EventArgs e)
        {
            // Gather text values and parse safely
            var newVitals = new VitalsModel
            {
                PatientID = _currentPatientId,
                RecordedAt = DateTime.UtcNow,
                Temperature = double.TryParse(TempEntry.Text, out var t) ? t : 0,
                SystolicBP = int.TryParse(SystolicEntry.Text, out var s) ? s : 0,
                DiastolicBP = int.TryParse(DiastolicEntry.Text, out var d) ? d : 0,
                HeartRate = int.TryParse(HeartRateEntry.Text, out var hr) ? hr : 0,
                Weight = double.TryParse(WeightEntry.Text, out var w) ? w : 0
            };

            // Call the service layer to POST to our backend API
            bool isSuccess = await _vitalsService.AddVitalsAsync(newVitals);

            if (isSuccess)
            {
                await DisplayAlertAsync("Success", "Vitals recorded successfully!", "OK");
                // Clear out the forms
                TempEntry.Text = SystolicEntry.Text = DiastolicEntry.Text = HeartRateEntry.Text = WeightEntry.Text = string.Empty;
            }
            else
            {
                await DisplayAlertAsync("Error", "Failed to save vitals entry. Please try again.", "OK");
            }
        }
=======
    // Go back to the previous page
    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
>>>>>>> main
    }
}