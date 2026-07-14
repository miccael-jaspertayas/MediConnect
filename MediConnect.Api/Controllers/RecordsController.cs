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

        public RecordsController(IRecordsRepository repository)
        {
            _repository = repository;
        }

        // Get all records for a patient
        [HttpGet("{patientId}")]
        public async Task<IActionResult> Get(int patientId)
        {
            var records = await _repository.GetByPatientId(patientId);
            return Ok(records);
        }

        // Get one record by RecordID
        [HttpGet("detail/{id}")]
        public async Task<IActionResult> GetRecord(int id)
        {
            var record = await _repository.GetById(id);

            if (record == null)
                return NotFound();

            return Ok(record);
        }

        // Add a new record
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] MedicalRecord record)
        {
            var created = await _repository.Add(record);
            return Ok(created);
        }

        // Update an existing record
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] MedicalRecord record)
        {
            if (id != record.RecordID)
                return BadRequest();

            await _repository.Update(record);
            return NoContent();
        }

        // Delete a record
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repository.Delete(id);
            return NoContent();
        }
    }
}