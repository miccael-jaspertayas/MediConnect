using MediConnect.Mobile.Services;

namespace MediConnect.Mobile.ViewModels
{
    public class VitalsViewModel : BaseViewModel // (or whatever base class you use)
    {
        private readonly VitalsService _vitalsService;

        // The dependency injection container automatically passes VitalsService here
        public VitalsViewModel(VitalsService vitalsService)
        {
            _vitalsService = vitalsService;
        }
    }
}