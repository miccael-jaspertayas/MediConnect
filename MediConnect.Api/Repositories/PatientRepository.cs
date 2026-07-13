using Microsoft.EntityFrameworkCore;
using MediConnect.Api.Data;
using MediConnect.Api.Models;

namespace MediConnect.Api.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly AppDbContext _db;

        public PatientRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Patient?> GetByIdAsync(int patientId)
        {
            return await _db.Patients.FirstOrDefaultAsync(p => p.PatientID == patientId);
        }

        public async Task UpdateAsync(Patient patient)
        {
            _db.Patients.Update(patient);
            await _db.SaveChangesAsync();
        }
    }
}
