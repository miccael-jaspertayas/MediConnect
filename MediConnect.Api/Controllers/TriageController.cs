using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediConnect.Api.Dtos;
using MediConnect.Api.Services;

namespace MediConnect.Api.Controllers
{
    [ApiController]
    [Route("api/triage")]
    public class TriageController : ControllerBase
    {
        private readonly TriageService _triageService;

        public TriageController(TriageService triageService)
        {
            _triageService = triageService;
        }

        [Authorize]
        [HttpPost("assess")]
        public IActionResult Assess([FromBody] TriageRequest request)
        {
            if (request.Symptoms == null || request.Symptoms.Count == 0)
            {
                return BadRequest("Please provide at least one symptom.");
            }

            var result = _triageService.Assess(request.Symptoms);
            return Ok(result);
        }
    }
}