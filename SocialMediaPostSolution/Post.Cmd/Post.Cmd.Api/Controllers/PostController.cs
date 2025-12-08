using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Cmd.Api.DTOs;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly ILogger<PostController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;
        public PostController(ILogger<PostController> logger, ICommandDispatcher commandDispatcher)
        {
            _logger = logger;
            _commandDispatcher = commandDispatcher;
        }

        [HttpPost]
        public async Task<ActionResult> NewPostAsync(NewPostCommand command)
        {
            var id = Guid.NewGuid();
            try
            {
                command.Id = id;
                await _commandDispatcher.DispatchAsync(command);
                return StatusCode(StatusCodes.Status201Created, new NewPostResponse { Id = id, Message = "New post creation request completed successfully." });
            }
            catch (InvalidOperationException ex)
            {
                _logger.Log(LogLevel.Warning, ex, "Client made a bad request");
                return BadRequest(new BaseResponse { Message = ex.Message });
            }
            catch (AggregateNotFoundException ex)
            {
                _logger.Log(LogLevel.Warning, ex, "Could not retreive aggregate, client passed an incorrect Post ID targetting the aggregate!");
                return BadRequest(new BaseResponse { Message = ex.Message });
            }
            catch (Exception ex) {
                const string SAFE_ERROR_MESSAGE = "Error while processing request to create a new post!";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);
                return StatusCode(StatusCodes.Status500InternalServerError, new NewPostResponse { Id = id, Message = SAFE_ERROR_MESSAGE });
            }
        }

        [HttpPost("{id}/like")]
        public async Task<ActionResult> LikePostAsync(LikePostCommand command)
        {
            try
            {
                await _commandDispatcher.DispatchAsync(command);
                return StatusCode(StatusCodes.Status204NoContent, new BaseResponse { Message = "Post liked." });
            }
            catch (InvalidOperationException ex)
            {
                _logger.Log(LogLevel.Warning, ex, "Client made a bad request");
                return BadRequest(new BaseResponse { Message = ex.Message });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "Error while processing request to like a post!";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse { Message = SAFE_ERROR_MESSAGE });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdatePostAsync(Guid id, EditMessageCommand command)
        {
            try
            {
                command.Id = id;
                await _commandDispatcher.DispatchAsync(command);
                return StatusCode(StatusCodes.Status204NoContent, new BaseResponse { Message = "Post updated" });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "Error while processing request to update a post!";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse { Message = SAFE_ERROR_MESSAGE });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePostAsync(Guid id, DeletePostCommand command)
        {
            try
            {
                command.Id=id;
                await _commandDispatcher.DispatchAsync(command);
                return StatusCode(StatusCodes.Status204NoContent, new BaseResponse { Message = "Post deleted" });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "Error while processing request to delete a post!";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse { Message = SAFE_ERROR_MESSAGE });
            }
        }
    }
}
