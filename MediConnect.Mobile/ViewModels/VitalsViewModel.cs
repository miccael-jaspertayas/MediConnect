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

        public ObservableCollection<VitalInsight> Insights { get; } = new();

        private bool _isAllNormal = true;
        public bool IsAllNormal { get => _isAllNormal; set => SetProperty(ref _isAllNormal, value); }

        // Colors for findings, by severity
        private static readonly Color HighBadge = Color.FromArgb("#FCE8E6");
        private static readonly Color HighText = Color.FromArgb("#C5221F");
        private static readonly Color LowBadge = Color.FromArgb("#FFF4E5");
        private static readonly Color LowText = Color.FromArgb("#B26A00");
        private static readonly Color NormalBadge = Color.FromArgb("#E6F4EA");
        private static readonly Color NormalText = Color.FromArgb("#137333");

        public void AnalyzeVitals(List<Vitals> vitalsList)
        {
            Insights.Clear();

            var latest = vitalsList?.OrderByDescending(v => v.RecordedAt).FirstOrDefault();

            if (latest == null)
            {
                IsAllNormal = true;
                return;
            }

            // Blood pressure. Checked as a category (systolic OR diastolic)
            if (latest.SystolicBP >= 140 || latest.DiastolicBP >= 90)
            {
                Insights.Add(new VitalInsight
                {
                    Label = "High Blood Pressure",
                    Detail = $"{latest.SystolicBP}/{latest.DiastolicBP} mmHg",
                    BadgeColor = HighBadge,
                    TextColor = HighText
                });
            }
            else if (latest.SystolicBP <= 90 || latest.DiastolicBP <= 60)
            {
                Insights.Add(new VitalInsight
                {
                    Label = "Low Blood Pressure",
                    Detail = $"{latest.SystolicBP}/{latest.DiastolicBP} mmHg",
                    BadgeColor = LowBadge,
                    TextColor = LowText
                });
            }

            // Temperature
            if (latest.Temperature >= 38.0)
            {
                Insights.Add(new VitalInsight
                {
                    Label = "Fever (High Temperature)",
                    Detail = $"{latest.Temperature:0.#}°C",
                    BadgeColor = HighBadge,
                    TextColor = HighText
                });
            }
            else if (latest.Temperature <= 35.5)
            {
                Insights.Add(new VitalInsight
                {
                    Label = "Low Temperature",
                    Detail = $"{latest.Temperature:0.#}°C",
                    BadgeColor = LowBadge,
                    TextColor = LowText
                });
            }

            // Heart rate
            if (latest.HeartRate >= 100)
            {
                Insights.Add(new VitalInsight
                {
                    Label = "Elevated Heart Rate",
                    Detail = $"{latest.HeartRate} bpm",
                    BadgeColor = HighBadge,
                    TextColor = HighText
                });
            }
            else if (latest.HeartRate <= 60)
            {
                Insights.Add(new VitalInsight
                {
                    Label = "Low Heart Rate",
                    Detail = $"{latest.HeartRate} bpm",
                    BadgeColor = LowBadge,
                    TextColor = LowText
                });
            }

            // SpO2
            if (latest.SpO2 > 0 && latest.SpO2 < 95)
            {
                Insights.Add(new VitalInsight
                {
                    Label = "Low Blood Oxygen",
                    Detail = $"{latest.SpO2:0.#}%",
                    BadgeColor = LowBadge,
                    TextColor = LowText
                });
            }

            IsAllNormal = Insights.Count == 0;
        }
    }
}