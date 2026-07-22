using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediConnect.Api.Dtos;
using MediConnect.Api.Models;
using MediConnect.Api.Services;

namespace MediConnect.Api.Controllers
{
    [ApiController]
    [Route("api/triage")]
    public class TriageController : ControllerBase
    {
        private readonly TriageService _triageService;
        private readonly TriageLogService _triageLogService;

        public TriageController(TriageService triageService, TriageLogService triageLogService)
        {
            _triageService = triageService;
            _triageLogService = triageLogService;
        }

        // Scores symptoms AND saves a new log entry in one step.
        [Authorize]
        [HttpPost("assess")]
        public async Task<IActionResult> Assess([FromBody] TriageRequest request)
        {
            if (request.Symptoms == null || request.Symptoms.Count == 0)
                return BadRequest("Please provide at least one symptom.");

            if (request.PatientID <= 0)
                return BadRequest("Invalid Patient ID.");

            var result = _triageService.Assess(request.Symptoms);

            var log = new TriageLog
            {
                PatientID = request.PatientID,
                Symptoms = string.Join(", ", request.Symptoms),
                Tier = result.Tier,
                Score = result.Score,
                Explanation = result.Explanation,
                CreatedAt = DateTime.UtcNow
            };
            var saved = await _triageLogService.AddLogAsync(log);

            var response = new TriageLogResponse
            {
                LogID = saved.LogID,
                Symptoms = request.Symptoms,
                Tier = result.Tier,
                Score = result.Score,
                Explanation = result.Explanation,
                CreatedAt = saved.CreatedAt
            };

            return Ok(response);
        }

        // Re-scores symptoms AND updates an existing log entry.
        [Authorize]
        [HttpPut("log/{id}")]
        public async Task<IActionResult> UpdateLog(int id, [FromBody] TriageRequest request)
        {
            if (request.Symptoms == null || request.Symptoms.Count == 0)
                return BadRequest("Please provide at least one symptom.");

            var existing = await _triageLogService.GetLogByIdAsync(id);
            if (existing is null || existing.PatientID != request.PatientID)
                return NotFound("Log entry not found.");

            var result = _triageService.Assess(request.Symptoms);

            existing.Symptoms = string.Join(", ", request.Symptoms);
            existing.Tier = result.Tier;
            existing.Score = result.Score;
            existing.Explanation = result.Explanation;

            var updated = await _triageLogService.UpdateLogAsync(existing);

            var response = new TriageLogResponse
            {
                LogID = updated.LogID,
                Symptoms = request.Symptoms,
                Tier = result.Tier,
                Score = result.Score,
                Explanation = result.Explanation,
                CreatedAt = updated.CreatedAt
            };

            return Ok(response);
        }

        [Authorize]
        [HttpGet("history/patient/{patientId}")]
        public async Task<IActionResult> GetHistory(int patientId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var (items, total) = await _triageLogService.GetLogsByPatientIdAsync(patientId, page, pageSize);

            var response = new PagedResult<TriageLogResponse>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = total,
                Items = items.Select(l => new TriageLogResponse
                {
                    LogID = l.LogID,
                    Symptoms = l.Symptoms.Split(", ", StringSplitOptions.RemoveEmptyEntries).ToList(),
                    Tier = l.Tier,
                    Score = l.Score,
                    Explanation = l.Explanation,
                    CreatedAt = l.CreatedAt
                }).ToList()
            };

            return Ok(response);
        }

        [Authorize]
        [HttpDelete("log/{id}")]
        public async Task<IActionResult> DeleteLog(int id, [FromQuery] int patientId)
        {
            var success = await _triageLogService.DeleteLogAsync(id, patientId);
            if (!success)
                return NotFound("Log entry not found.");

            return NoContent();
        }

        [Authorize]
        [HttpDelete("history/patient/{patientId}")]
        public async Task<IActionResult> ClearHistory(int patientId)
        {
            await _triageLogService.DeleteAllForPatientAsync(patientId);
            return NoContent();
        }
    }
}