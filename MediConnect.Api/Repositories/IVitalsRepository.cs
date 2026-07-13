using System.Collections.Generic;
using System.Threading.Tasks;
using MediConnect.Api.Models;

namespace MediConnect.Api.Repositories
{
    public interface IVitalsRepository
    {
        Task<IEnumerable<Vitals>> GetVitalsByPatientIdAsync(int patientId);
        Task<Vitals> AddVitalsAsync(Vitals vitals);
    }
}