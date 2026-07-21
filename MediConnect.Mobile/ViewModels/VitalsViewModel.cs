using System.Collections.ObjectModel;
using System.Windows.Input;
using MediConnect.Mobile.Models;
using MediConnect.Mobile.Services;

namespace MediConnect.Mobile.ViewModels
{
    public class VitalsViewModel : BaseViewModel
    {
        private readonly VitalsService _vitalsService;
        private readonly SessionService _sessionService;

        public ObservableCollection<Vitals> Vitals { get; } = new ObservableCollection<Vitals>();

        public ICommand LoadVitalsCommand { get; }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public VitalsViewModel(VitalsService vitalsService, SessionService sessionService)
        {
            _vitalsService = vitalsService;
            _sessionService = sessionService;

            LoadVitalsCommand = new Command(async () => await LoadVitalsAsync());
        }

        public async Task LoadVitalsAsync()
        {
            if (IsBusy) return;

            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                Vitals.Clear();

                var vitals = await _vitalsService.GetVitalsAsync(_sessionService.PatientID);

                vitals = vitals
                    .OrderByDescending(v => v.RecordedAt)
                    .ToList();

                foreach (var item in vitals)
                    Vitals.Add(item);

                AnalyzeVitals(vitals);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task DeleteVitalAsync(Vitals vital)
        {
            if (IsBusy) return;

            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                var success = await _vitalsService.DeleteVitalsAsync(vital.VitalID);

                if (success)
                    Vitals.Remove(vital);
                else
                    ErrorMessage = "Unable to delete record.";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private string _vitalsAnalysis = "No recent data to analyze.";
        public string VitalsAnalysis
        {
            get => _vitalsAnalysis;
            set => SetProperty(ref _vitalsAnalysis, value);
        }

        private Color _analysisBadgeColor = Color.FromArgb("#E6F4EA");
        public Color AnalysisBadgeColor
        {
            get => _analysisBadgeColor;
            set => SetProperty(ref _analysisBadgeColor, value);
        }

        private Color _analysisTextColor = Color.FromArgb("#137333");
        public Color AnalysisTextColor
        {
            get => _analysisTextColor;
            set => SetProperty(ref _analysisTextColor, value);
        }

        public void AnalyzeVitals(List<Vitals> vitalsList)
        {
            if (vitalsList == null || !vitalsList.Any())
            {
                VitalsAnalysis = "No vitals recorded yet. Start logging to see insights.";
                AnalysisBadgeColor = Colors.Gray;
                return;
            }

            // Get the most recent entry
            var latest = vitalsList.OrderByDescending(v => v.RecordedAt).FirstOrDefault();

            if (latest == null) return;

            // Evaluate based on standard clinical thresholds
            bool highBP = (latest.SystolicBP >= 140 || latest.DiastolicBP >= 90);
            bool highTemp = latest.Temperature >= 38.0;
            bool highHR = latest.HeartRate >= 100;
            bool lowSpO2 = latest.SpO2 > 0 && latest.SpO2 < 95;

            if (highBP || highTemp || highHR || lowSpO2)
            {
                var issues = new List<string>();
                if (highBP) issues.Add("Elevated blood pressure parameters");
                if (highTemp) issues.Add("Pyrexia-range temperature");
                if (highHR) issues.Add("Elevated heart rate");
                if (lowSpO2) issues.Add("Low blood oxygen saturation (SpO2)");

                VitalsAnalysis = $"Clinical Notice: The most recent assessment indicates {string.Join(", ", issues)}. Clinical evaluation is advised.";
                AnalysisBadgeColor = Color.FromArgb("#FCE8E6"); // Soft red background
                AnalysisTextColor = Color.FromArgb("#C5221F");  // Dark red text
            }
            else
            {
                VitalsAnalysis = "All recorded parameters remain within normal clinical limits.";
                AnalysisBadgeColor = Color.FromArgb("#E6F4EA"); // Soft mint green background
                AnalysisTextColor = Color.FromArgb("#137333");  // Dark green text
            }
        }
    }
}