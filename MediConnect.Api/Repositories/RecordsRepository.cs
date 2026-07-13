using Microsoft.EntityFrameworkCore;
using MediConnect.Api.Data;
using MediConnect.Api.Models;

namespace MediConnect.Api.Repositories
{
    public class RecordsRepository : IRecordsRepository
    {
        private readonly AppDbContext _context;

        public RecordsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<MedicalRecord>>
            GetByPatientId(int patientId)
        {
            return await _context.MedicalRecords
                .Where(r => r.PatientID == patientId)
                .OrderByDescending(r => r.VisitDate)
                .ToListAsync();
        }

        public async Task<MedicalRecord>
            Add(MedicalRecord record)
        {
            _context.MedicalRecords.Add(record);
            await _context.SaveChangesAsync();
            return record;
        }

        public async Task Update(MedicalRecord record)
        {
            _context.MedicalRecords.Update(record);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var record =
                await _context.MedicalRecords.FindAsync(id);

            if (record != null)
            {
                _context.MedicalRecords.Remove(record);
                await _context.SaveChangesAsync();
            }
        }
    }
}