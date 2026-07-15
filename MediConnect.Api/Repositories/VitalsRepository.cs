using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MediConnect.Api.Models;
using MediConnect.Api.Data; 

namespace MediConnect.Api.Repositories
{
    public class VitalsRepository : IVitalsRepository
    {
        
        private readonly AppDbContext _context;

        
        public VitalsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Vitals>> GetByPatientIdAsync(int patientId)
        {
            return await _context.Vitals
                .Where(v => v.PatientID == patientId)
                .OrderByDescending(v => v.RecordedAt)
                .ToListAsync();
        }

        public async Task<Vitals> AddAsync(Vitals vitals)
        {
            _context.Vitals.Add(vitals);
            await _context.SaveChangesAsync();
            return vitals;
        }

        public async Task<bool> UpdateAsync(Vitals vitals)
        {
            _context.Vitals.Update(vitals);
            var written = await _context.SaveChangesAsync();
            return written > 0;
        }

        public async Task<bool> DeleteAsync(int vitalId)
        {
            var vital = await _context.Vitals.FindAsync(vitalId);
            if (vital == null) return false;

            _context.Vitals.Remove(vital);
            var written = await _context.SaveChangesAsync();
            return written > 0;
        }
    }
}