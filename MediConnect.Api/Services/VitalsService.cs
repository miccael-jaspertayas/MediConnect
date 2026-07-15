using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediConnect.Api.Models; // Adjust this to where your backend Vitals model lives
using MediConnect.Api.Repositories; // Adjust this to your Repository namespace

namespace MediConnect.Api.Services
{
    public class VitalsService
    {
        // Assuming you have a Repository pattern set up based on your folder structure
        private readonly IVitalsRepository _vitalsRepository;

        public VitalsService(IVitalsRepository vitalsRepository)
        {
            _vitalsRepository = vitalsRepository;
        }

        public async Task<IEnumerable<Vitals>> GetVitalsByPatientIdAsync(int patientId)
        {
            return await _vitalsRepository.GetByPatientIdAsync(patientId);
        }

        public async Task<Vitals?> AddVitalsAsync(Vitals vitals)
        {
            vitals.RecordedAt = DateTime.UtcNow; // Ensure consistent timestamps
            return await _vitalsRepository.AddAsync(vitals);
        }

        public async Task<bool> UpdateVitalsAsync(Vitals vitals)
        {
            return await _vitalsRepository.UpdateAsync(vitals);
        }

        public async Task<bool> DeleteVitalsAsync(int vitalId)
        {
            return await _vitalsRepository.DeleteAsync(vitalId);
        }
    }
}