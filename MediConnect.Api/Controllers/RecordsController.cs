using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediConnect.Api.Models;
using MediConnect.Api.Repositories;

namespace MediConnect.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RecordsController : ControllerBase
    {
        private readonly IRecordsRepository _repository;

        public RecordsController(
            IRecordsRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("{patientId}")]
        public async Task<IActionResult>
            Get(int patientId)
        {
            var records =
                await _repository.GetByPatientId(patientId);

            return Ok(records);
        }

        [HttpPost]
        public async Task<IActionResult>
            Post([FromBody] MedicalRecord record)
        {
            var created =
                await _repository.Add(record);

            return Ok(created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult>
            Put(int id,
                [FromBody] MedicalRecord record)
        {
            if (id != record.RecordID)
                return BadRequest();

            await _repository.Update(record);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult>
            Delete(int id)
        {
            await _repository.Delete(id);

            return NoContent();
        }
    }
}