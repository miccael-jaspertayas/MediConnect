using MediConnect.Api.Models;
using MediConnect.Api.Repositories;

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MediConnectLite.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VitalsController : ControllerBase
    {
        private readonly IVitalsRepository _repository;

        public VitalsController(IVitalsRepository repository)
        {
            _repository = repository;
        }

        // POST: api/vitals
        [HttpPost]
        public async Task<IActionResult> AddVitals([FromBody] Vitals vitals)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var createdVitals = await _repository.AddVitalsAsync(vitals);
            return Ok(createdVitals);
        }

        // GET: api/vitals/patient/{patientId}
        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetVitalsByPatient(int patientId)
        {
            var vitalsList = await _repository.GetVitalsByPatientIdAsync(patientId);
            return Ok(vitalsList);
        }
    }
}