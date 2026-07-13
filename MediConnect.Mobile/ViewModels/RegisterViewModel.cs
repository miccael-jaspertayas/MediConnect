using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using MediConnect.Mobile.Services;
using MediConnect.Mobile.ViewModels;

namespace MediConnect.Mobile.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly AuthService _authService;

        private string _email = string.Empty;
        public string Email { get => _email; set => SetProperty(ref _email, value); }

        private string _password = string.Empty;
        public string Password { get => _password; set => SetProperty(ref _password, value); }

        private string _statusMessage = string.Empty;
        public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }

        public ICommand RegisterCommand { get; }

        public RegisterViewModel(AuthService authService)
        {
            _authService = authService;
            RegisterCommand = new Command(async () => await RegisterAsync());
        }

        private async Task RegisterAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var success = await _authService.RegisterAsync(Email, Password);
                StatusMessage = success
                    ? "Account created! Please log in."
                    : "That email is already registered.";

                if (success)
                    await Shell.Current.GoToAsync("//Login");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
