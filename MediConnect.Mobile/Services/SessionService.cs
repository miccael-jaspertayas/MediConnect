using Microsoft.Maui.Storage;
using System;
using System.Threading.Tasks;
using MediConnect.Mobile.Models;

namespace MediConnect.Mobile.Services
{
    // Registered as a Singleton in MauiProgram.cs
    public class SessionService
    {
        public string? Token { get; private set; }
        public int UserID { get; private set; }
        public int PatientID { get; private set; }

        public bool IsLoggedIn => !string.IsNullOrEmpty(Token);

        
        public VitalsModel? MostRecentVital { get; private set; }

        
        public event Action<VitalsModel>? OnVitalsUpdated;

        public void UpdateMostRecentVital(VitalsModel vital)
        {
            MostRecentVital = vital;
            OnVitalsUpdated?.Invoke(vital);
        }
        

        public void SetSession(string token, int userId, int patientId)
        {
            Token = token;
            UserID = userId;
            PatientID = patientId;
        }

        public async Task PersistAsync()
        {
            if (Token is null) return;
            await SecureStorage.SetAsync("jwt_token", Token);
            await SecureStorage.SetAsync("user_id", UserID.ToString());
            await SecureStorage.SetAsync("patient_id", PatientID.ToString());
        }

        public async Task<bool> TryRestoreAsync()
        {
            var token = await SecureStorage.GetAsync("jwt_token");
            var userId = await SecureStorage.GetAsync("user_id");
            var patientId = await SecureStorage.GetAsync("patient_id");

            if (token is null || userId is null || patientId is null) return false;

            Token = token;
            UserID = int.Parse(userId);
            PatientID = int.Parse(patientId);
            return true;
        }

        public void Clear()
        {
            Token = null;
            UserID = 0;
            PatientID = 0;
            MostRecentVital = null; 
            SecureStorage.RemoveAll();
        }
    }

    
    
}