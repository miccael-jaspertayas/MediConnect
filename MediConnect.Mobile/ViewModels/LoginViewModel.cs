using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using MediConnect.Mobile.Services;
using MediConnect.Mobile.ViewModels;

namespace MediConnect.Mobile.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly AuthService _authService;

        private string _email = string.Empty;
        public string Email { get => _email; set => SetProperty(ref _email, value); }

        private string _password = string.Empty;
        public string Password { get => _password; set => SetProperty(ref _password, value); }

        private string _errorMessage = string.Empty;
        public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }

        public ICommand LoginCommand { get; }

        public LoginViewModel(AuthService authService)
        {
            _authService = authService;
            LoginCommand = new Command(async () => await LoginAsync());
        }

        private async Task LoginAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                var success = await _authService.LoginAsync(Email, Password);
                if (success)
                {
                    await Shell.Current.GoToAsync("//Dashboard");
                }
                else
                {
                    ErrorMessage = "Invalid email or password.";
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
