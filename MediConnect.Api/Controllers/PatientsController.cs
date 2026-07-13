using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediConnect.Api.Dtos;
using MediConnect.Api.Repositories;

namespace MediConnect.Api.Controllers
{
    [ApiController]
    [Route("api/patients")]
    [Authorize]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientRepository _patientRepository;

        public PatientsController(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        private bool IsOwnPatientId(int id)
        {
            var claim = User.FindFirst("PatientID")?.Value;
            return claim != null && int.Parse(claim) == id;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (!IsOwnPatientId(id)) return Forbid();

            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient is null) return NotFound();

            var dto = new PatientDto
            {
                PatientID = patient.PatientID,
                Name = patient.Name,
                DOB = patient.DOB,
                BloodType = patient.BloodType,
                Allergies = patient.Allergies,
                Medications = patient.Medications,
                EmergencyContactName = patient.EmergencyContactName,
                EmergencyContactPhone = patient.EmergencyContactPhone
            };

            return Ok(dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, PatientDto dto)
        {
            if (!IsOwnPatientId(id)) return Forbid();

            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient is null) return NotFound();

            patient.Name = dto.Name;
            patient.DOB = dto.DOB;
            patient.BloodType = dto.BloodType;
            patient.Allergies = dto.Allergies;
            patient.Medications = dto.Medications;
            patient.EmergencyContactName = dto.EmergencyContactName;
            patient.EmergencyContactPhone = dto.EmergencyContactPhone;

            await _patientRepository.UpdateAsync(patient);
            return Ok(dto);
        }
    }
}
