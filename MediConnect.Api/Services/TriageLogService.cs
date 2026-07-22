using MediConnect.Api.Data;
using MediConnect.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace MediConnect.Api.Services
{
    public class TriageLogService
    {
        private readonly AppDbContext _context;

        public TriageLogService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TriageLog> AddLogAsync(TriageLog log)
        {
            _context.TriageLogs.Add(log);
            await _context.SaveChangesAsync();
            return log;
        }

        public async Task<(List<TriageLog> Items, int TotalCount)> GetLogsByPatientIdAsync(int patientId, int page, int pageSize)
        {
            var query = _context.TriageLogs
                .Where(t => t.PatientID == patientId)
                .OrderByDescending(t => t.CreatedAt);

            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (items, total);
        }

        public async Task<TriageLog?> GetLogByIdAsync(int logId)
        {
            return await _context.TriageLogs.FindAsync(logId);
        }

        public async Task<TriageLog> UpdateLogAsync(TriageLog log)
        {
            _context.TriageLogs.Update(log);
            await _context.SaveChangesAsync();
            return log;
        }

        public async Task<bool> DeleteLogAsync(int logId, int patientId)
        {
            var log = await _context.TriageLogs.FirstOrDefaultAsync(t => t.LogID == logId && t.PatientID == patientId);
            if (log is null) return false;

            _context.TriageLogs.Remove(log);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> DeleteAllForPatientAsync(int patientId)
        {
            var logs = _context.TriageLogs.Where(t => t.PatientID == patientId);
            int count = await logs.CountAsync();
            _context.TriageLogs.RemoveRange(logs);
            await _context.SaveChangesAsync();
            return count;
        }
    }
}