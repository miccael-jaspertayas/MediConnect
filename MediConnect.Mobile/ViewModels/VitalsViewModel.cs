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
    }
}