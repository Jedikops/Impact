using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TendersApi.App.Common;
using TendersApi.App.Handlers;
using TendersApi.App.Queries;
using TendersApi.Domain;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TendersApi.WebApi.Controllers
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
        public async Task<IActionResult> Get([FromQuery] int page = 1,
            [FromQuery] DateTime? before = null,
            [FromQuery] DateTime? after = null,
            [FromQuery] OrderBy orderBy = OrderBy.NotSet,
            [FromQuery] OrderByDirection orderByDirection = OrderByDirection.Ascending)
        {

            if(!Enum.IsDefined(typeof(OrderBy), orderBy))
            {
                return BadRequest();
            }

            var result = await _getTendersHandler.Handle(
                new GetTendersQuery
                {
                    Page = page,
                    Before = before,
                    After = after,
                    OrderBy = orderBy, 
                    OrderByDirection = orderByDirection
                });

            if (result.IsSuccess)
                return Ok(result);

            return result.Status switch
            {
                ResultStatus.ValidationError => BadRequest(result),
                ResultStatus.NotFound => NotFound(result),
                ResultStatus.ExternalApiError => StatusCode(502, result),
                _ => StatusCode(500, result),
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
