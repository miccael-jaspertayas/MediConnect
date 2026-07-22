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
            await _triageLogService.AddLogAsync(log);

            return Ok(result);
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
    }
}