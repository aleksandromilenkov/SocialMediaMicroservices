using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Cmd.Api.DTOs;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestoreReadDbController(ICommandDispatcher _dispatcher, ILogger<RestoreReadDbController> _logger) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> RestoreReadDbAsync()
        {
            try
            {
                await _dispatcher.DispatchAsync(new RestoreReadDbCommand());
                return StatusCode(StatusCodes.Status201Created, new BaseResponse() { Message = "Read database restore request completed successfully" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.Log(LogLevel.Warning, ex, "Client made a bad request");
                return BadRequest(new BaseResponse() { Message = ex.Message });
            }
            catch (Exception ex) {
                const string SAFE_ERROR_MESSAGE = "Error while processing request to restore a read database";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }
    }
}
