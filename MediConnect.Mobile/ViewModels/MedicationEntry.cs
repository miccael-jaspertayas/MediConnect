using MediConnect.Mobile.Models;

namespace MediConnect.Mobile.ViewModels
{
    public class MedicationEntry : BaseViewModel
    {
        private string _name = string.Empty;
        public string Name { get => _name; set => SetProperty(ref _name, value); }

        private MedicationLookupResult? _lookupResult;
        public MedicationLookupResult? LookupResult
        {
            get => _lookupResult;
            set => SetProperty(ref _lookupResult, value);
        }

        private string _lookupStatusMessage = string.Empty;
        public string LookupStatusMessage
        {
            get => _lookupStatusMessage;
            set => SetProperty(ref _lookupStatusMessage, value);
        }
    }
}