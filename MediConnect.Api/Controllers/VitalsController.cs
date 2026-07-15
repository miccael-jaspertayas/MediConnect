using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MediConnect.Api.Models;
using MediConnect.Api.Services;

namespace MediConnect.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VitalsController : ControllerBase
    {
        private readonly VitalsService _vitalsService;

        public VitalsController(VitalsService vitalsService)
        {
            _vitalsService = vitalsService;
        }

        // GET: api/vitals/patient/{patientId}
        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetByPatient(int patientId)
        {
            var vitals = await _vitalsService.GetVitalsByPatientIdAsync(patientId);
            return Ok(vitals);
        }

        // POST: api/vitals
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Vitals vitals)
        {
            if (vitals == null)
            {
                return BadRequest("Vitals data is null.");
            }

            // Ensure PatientID is valid and coming through
            if (vitals.PatientID <= 0)
            {
                return BadRequest("Invalid Patient ID.");
            }

            var created = await _vitalsService.AddVitalsAsync(vitals);

            if (created == null)
            {
                return BadRequest("Could not record vitals in database.");
            }

            // Return 201 Created Status
            return CreatedAtAction(nameof(GetByPatient), new { patientId = created.PatientID }, created);
        }

        // PUT: api/vitals/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Vitals vitals)
        {
            if (id != vitals.VitalID) return BadRequest("ID mismatch.");

            var success = await _vitalsService.UpdateVitalsAsync(vitals);
            if (!success) return NotFound("Record not found or no changes made.");
            return NoContent();
        }

        // DELETE: api/vitals/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _vitalsService.DeleteVitalsAsync(id);
            if (!success) return NotFound("Record not found.");
            return NoContent();
        }
    }
}