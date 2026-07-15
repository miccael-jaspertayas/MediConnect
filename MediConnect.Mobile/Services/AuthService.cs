using System;
using System.Collections.Generic;
using System.Text;
using MediConnect.Mobile.Dtos;

namespace MediConnect.Mobile.Services
{
    public class AuthService
    {
        private readonly ApiService _api;
        private readonly SessionService _session;

        public AuthService(ApiService api, SessionService session)
        {
            _api = api;
            _session = session;
        }

        public async Task<bool> RegisterAsync(string email, string password)
        {
            var result = await _api.PostAsync<RegisterRequest, object>(
                "api/auth/register",
                new RegisterRequest
                {
                    Email = email,
                    Password = password
                });

            return result != null;
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            var result = await _api.PostAsync<LoginRequest, LoginResponse>(
                "api/auth/login",
                new LoginRequest
                {
                    Email = email,
                    Password = password
                });

            if (result is null)
                return false;

            _session.SetSession(
                result.Token,
                result.UserID,
                result.PatientID
            );

            await _session.PersistAsync();

            return true;
        }
    }
}