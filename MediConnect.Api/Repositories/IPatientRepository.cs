using MediConnect.Api.Models;

namespace MediConnect.Api.Repositories
{
    public interface IPatientRepository
    {
        Task<Patient?> GetByIdAsync(int patientId);
        Task UpdateAsync(Patient patient);
    }
}
