using MediConnect.Api.Models;

namespace MediConnect.Api.Repositories
{
    public interface IRecordsRepository
    {
        Task<List<MedicalRecord>> GetByPatientId(int patientId);
        Task<MedicalRecord?> GetById(int id);
        Task<MedicalRecord> Add(MedicalRecord record);
        Task Update(MedicalRecord record);
        Task Delete(int id);
    }
}