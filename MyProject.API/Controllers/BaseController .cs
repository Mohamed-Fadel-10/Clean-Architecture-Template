using Domain.Enums;
using Domain.ResultPattern;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Text.Json;
using Domain.Enums.ResultPattern;
using Shared.Pagination;

namespace API.Controllers
{
    /// <summary>
    /// Base controller class that provides common functionality for API controllers.
    /// </summary>
    /// <param name="mediator">The mediator instance for handling requests.</param>
    /// <param name="errorMessageService">The service for handling error messages.</param>
    [ApiController]
    [Route("api/v1/[controller]")]
    public abstract class BaseController(IMediator mediator) : ControllerBase
    {

        /// <summary>
        /// The mediator instance used for handling requests.
        /// </summary>
        protected readonly IMediator Mediator = mediator;

        /// <summary>
        /// Adds pagination metadata to the response headers.
        /// </summary>
        /// <param name="paginationMetaData">The pagination metadata to be added to the response headers.</param>
        [NonAction]
        private void AddPaginationHeader(PaginationMetaData paginationMetaData)
        {
            Response.Headers.Add(new KeyValuePair<string, StringValues>("X-Pagination", JsonSerializer.Serialize(paginationMetaData)));
        }
        protected ActionResult FromResult<T>(Result<T> result)
        {
            return result.Status switch
            {
                ResultStatus.Success => Ok(result),
                ResultStatus.NotFound => NotFound(result),
                ResultStatus.Unauthorized => StatusCode(403, result),
                ResultStatus.Error => BadRequest(result),
                ResultStatus.Forbidden => StatusCode(403, result),
                ResultStatus.Conflict => BadRequest(result),
                ResultStatus.BadRequest => BadRequest(result),
                _ => throw new Exception("An unhandled result has occurred as a result of a service call."),
            };
        }

        protected ActionResult FromPagedResult<T>(Result<PagedList<T>> result)
        {
            if (result.Data is not null)
                AddPaginationHeader(result.Data.MetaData);
            return result.Status switch
            {
                ResultStatus.Success => Ok(result),
                ResultStatus.NotFound => NotFound(result),
                ResultStatus.Forbidden => Unauthorized(result),
                ResultStatus.Conflict => BadRequest(result),
                ResultStatus.Unauthorized => Unauthorized(result),
                ResultStatus.Error => BadRequest(result),
                ResultStatus.BadRequest => BadRequest(result),
                _ => throw new Exception("An unhandled result has occurred as a result of a service call."),
            };
        }
    }
}
