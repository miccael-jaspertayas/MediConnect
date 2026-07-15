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

        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            
            return Ok();
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

            if (vitals.PatientID <= 0)
            {
                return BadRequest("Invalid Patient ID.");
            }

            // Explicitly ensure the ID is 0 so EF Core treats it as a brand new database row insert
            vitals.VitalID = 0;

            var created = await _vitalsService.AddVitalsAsync(vitals);

            if (created == null)
            {
                return BadRequest("Could not record vitals in database.");
            }

            // Now targets our new GetById route perfectly without throwing a routing exception
            return CreatedAtAction(nameof(GetById), new { id = created.VitalID }, created);
        }

        // PUT: api/vitals/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Vitals vitals)
        {
            if (vitals == null || id != vitals.VitalID)
            {
                return BadRequest("ID mismatch.");
            }

            var success = await _vitalsService.UpdateVitalsAsync(vitals);
            if (!success)
            {
                return NotFound("Record not found or no changes made.");
            }

            return NoContent();
        }

        // DELETE: api/vitals/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _vitalsService.DeleteVitalsAsync(id);
            if (!success)
            {
                return NotFound("Record not found.");
            }

            return NoContent();
        }
    }
}