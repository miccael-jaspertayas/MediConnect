using System.Collections.Generic;
using System.Threading.Tasks;
using MediConnect.Api.Models;

namespace MediConnect.Api.Repositories
{
    public interface IVitalsRepository
    {
        Task<IEnumerable<Vitals>> GetByPatientIdAsync(int patientId);
        Task<Vitals> AddAsync(Vitals vitals);
        Task<bool> UpdateAsync(Vitals vitals);
        Task<bool> DeleteAsync(int vitalId);
    }
}