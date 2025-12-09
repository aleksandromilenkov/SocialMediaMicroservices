using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Cmd.Api.DTOs;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ILogger<PostController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;
        public CommentController(ILogger<PostController> logger, ICommandDispatcher commandDispatcher)
        {
            _logger = logger;
            _commandDispatcher = commandDispatcher;
        }


        [HttpPost("{id}")]
        public async Task<ActionResult> NewCommentAsync(Guid id, [FromBody] AddCommentCommand command)
        {
            try
            {
                command.Id = id;
                await _commandDispatcher.DispatchAsync(command);
                return StatusCode(StatusCodes.Status201Created, new BaseResponse { Message = "New comment creation request completed successfully." });
            }
            catch (InvalidOperationException ex)
            {
                _logger.Log(LogLevel.Warning, ex, "Client made a bad request");
                return BadRequest(new BaseResponse { Message = ex.Message });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "Error while processing request to create a new comment!";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);
                return StatusCode(StatusCodes.Status500InternalServerError, new NewCommentResponse { Id = id, Message = SAFE_ERROR_MESSAGE });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCommentAsync(Guid id, [FromBody] EditCommentCommand command)
        {
            try
            {
                command.Id = id;
                await _commandDispatcher.DispatchAsync(command);
                return StatusCode(StatusCodes.Status204NoContent, new BaseResponse { Message = "Comment updated" });
            }
            catch (AggregateNotFoundException ex)
            {
                _logger.Log(LogLevel.Warning, ex, "Could not retreive aggregate, client passed an incorrect Post ID targetting the aggregate!");
                return BadRequest(new BaseResponse { Message = ex.Message });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "Error while processing request to update a comment!";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse { Message = SAFE_ERROR_MESSAGE });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCommentAsync(Guid id, [FromBody] RemoveCommentCommand command)
        {
            try
            {
                command.Id = id;
                await _commandDispatcher.DispatchAsync(command);
                return StatusCode(StatusCodes.Status204NoContent, new BaseResponse { Message = "Comment deleted" });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "Error while processing request to delete a comment!";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse { Message = SAFE_ERROR_MESSAGE });
            }
        }
    }
}
