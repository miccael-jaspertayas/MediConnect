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

        private bool _isRegisterSuccess;
        public bool IsRegisterSuccess
        {
            get => _isRegisterSuccess;
            set => SetProperty(ref _isRegisterSuccess, value);
        }

        private bool _isPasswordHidden = true;
        public bool IsPasswordHidden { get => _isPasswordHidden; set => SetProperty(ref _isPasswordHidden, value); }

        private bool _isConfirmPasswordHidden = true;
        public bool IsConfirmPasswordHidden { get => _isConfirmPasswordHidden; set => SetProperty(ref _isConfirmPasswordHidden, value); }

        private string _confirmPassword = string.Empty;
        public string ConfirmPassword { get => _confirmPassword; set => SetProperty(ref _confirmPassword, value); }

        public ICommand TogglePasswordCommand { get; }
        public ICommand ToggleConfirmPasswordCommand { get; }

        public ICommand RegisterCommand { get; }

        public RegisterViewModel(AuthService authService)
        {
            _authService = authService;
            RegisterCommand = new Command(async () => await RegisterAsync());
            TogglePasswordCommand = new Command(() => IsPasswordHidden = !IsPasswordHidden);
            ToggleConfirmPasswordCommand = new Command(() => IsConfirmPasswordHidden = !IsConfirmPasswordHidden);
        }

        private async Task RegisterAsync()
        {
            if (IsBusy) return;
            StatusMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                IsRegisterSuccess = false;
                StatusMessage = "Email and password are required.";
                return;
            }

            if (Password != ConfirmPassword)
            {
                IsRegisterSuccess = false;
                StatusMessage = "Passwords do not match.";
                return;
            }

            IsBusy = true;
            try
            {
                var success = await _authService.RegisterAsync(Email, Password);
                IsRegisterSuccess = success;
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
