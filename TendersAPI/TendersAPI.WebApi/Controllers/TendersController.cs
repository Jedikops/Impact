using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
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
        private readonly GetTendersBySupplierIdQueryHandler _getTendersBySupplierIdQueryHandler;

        public TendersController(GetTendersQueryHandler getTendersHandler, GetTenderByIdQueryHandler getTenderByIdHandler, GetTendersBySupplierIdQueryHandler getTendersBySupplierIdQueryHandler)
        {
            _getTendersHandler = getTendersHandler;
            _getTenderByIdHandler = getTenderByIdHandler;
            _getTendersBySupplierIdQueryHandler = getTendersBySupplierIdQueryHandler;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int page = 1,
            [FromQuery] DateTime? before = null,
            [FromQuery] DateTime? after = null,
            [FromQuery] int lessThan = int.MaxValue,
            [FromQuery] int greaterThan = 0,
            [FromQuery] OrderBy orderBy = OrderBy.NotSet,
            [FromQuery] OrderByDirection orderByDirection = OrderByDirection.Ascending)
        {
            var result = await _getTendersHandler.Handle(
                new GetTendersQuery
                {
                    Page = page,
                    Before = before,
                    After = after,
                    LessThan = lessThan,
                    GreaterThan = greaterThan,
                    OrderBy = orderBy,
                    OrderByDirection = orderByDirection
                });

            return HandleResult(result);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _getTenderByIdHandler.Handle(new GetTenderByIdQuery { Id = id });

            return HandleResult(result);
        }

        [HttpGet("supplier/{id}")]
        public async Task<IActionResult> GetBySupplierId(int id)
        {
            var result = await _getTendersBySupplierIdQueryHandler.Handle(new GetTendersBySupplierIdQuery { Id = id });

            return HandleResult(result);
        }

        private IActionResult HandleResult<T>(Result<T> result)
        {
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
    }
}
