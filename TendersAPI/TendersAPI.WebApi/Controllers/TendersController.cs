using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TendersApi.App.Common;
using TendersApi.App.Handlers;
using TendersApi.App.Queries;
using static System.Net.Mime.MediaTypeNames;

namespace TendersAPI.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TendersController : ControllerBase
    {
        private readonly GetTendersQueryHandler _getTendersHandler;
        private readonly GetTenderByIdQueryHandler _getTenderByIdHandler;

        public TendersController(GetTendersQueryHandler getTendersHandler, GetTenderByIdQueryHandler getTenderByIdHandler)
        {
            _getTendersHandler = getTendersHandler;
            _getTenderByIdHandler = getTenderByIdHandler;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int page = 1)
        {
            var result = await _getTendersHandler.Handle(new GetTendersQuery { Page = page});

            if (result.IsSuccess)
                return Ok(result);

            return result.Status switch
            {
                ResultStatus.ValidationError => BadRequest(new { error = result.Error }),
                ResultStatus.NotFound => NotFound(new { error = result.Error }),
                ResultStatus.ExternalApiError => StatusCode(502, new { error = result.Error }),
                _ => StatusCode(500, new { error = "Internal server error" }),
            };
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var tender = await _getTenderByIdHandler.Handle(new GetTenderByIdQuery { Id = id });
            if (tender == null) return NotFound();
            return Ok(tender);
        }

    }
}
