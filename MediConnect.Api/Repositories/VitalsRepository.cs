using MediConnect.Api.Data;
using MediConnect.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediConnect.Api.Repositories
{
    public class VitalsRepository : IVitalsRepository
    {
        private readonly AppDbContext _context;

        // Dependency Injection handles providing the DbContext
        public VitalsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Vitals>> GetVitalsByPatientIdAsync(int patientId)
        {
            return await _context.Vitals
                .Where(v => v.PatientID == patientId)
                .OrderByDescending(v => v.RecordedAt)
                .ToListAsync();
        }

        public async Task<Vitals> AddVitalsAsync(Vitals vitals)
        {
            _context.Vitals.Add(vitals);
            await _context.SaveChangesAsync();
            return vitals;
        }
    }
}