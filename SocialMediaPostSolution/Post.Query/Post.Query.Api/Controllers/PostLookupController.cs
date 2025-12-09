using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Post.Common.DTOs;
using Post.Query.Api.DTOs;
using Post.Query.Api.Queries;
using Post.Query.Domain.Entities;

namespace Post.Query.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PostLookupController : ControllerBase
    {
        private readonly ILogger<PostLookupController> _logger;
        private readonly IQueryDispatcher<PostEntity> _queryDispatcher;

        public PostLookupController(ILogger<PostLookupController> logger, IQueryDispatcher<PostEntity> queryDispatcher)
        {
            _logger = logger;
            _queryDispatcher = queryDispatcher;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllPostsAsync()
        {
            try
            {
                var query = new FindAllPostsQuery();
                var posts = await _queryDispatcher.DispatchAsync(query);
                if (posts == null || posts.Count == 0) return NoContent();
                return Ok(new PostLookupResponse
                {
                    Posts = posts,
                    Message = $"Successfully returned {posts.Count} post{(posts.Count > 1 ? "s" : string.Empty)}",
                });
            }
            catch (Exception ex) {
                const string SAFE_ERROR_MESSAGE = "Error while processing request to retreive posts";
                _logger.LogError(ex, SAFE_ERROR_MESSAGE);
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse {  Message = SAFE_ERROR_MESSAGE });
            }
        }


        [HttpGet("author/{authorName}")]
        public async Task<ActionResult> GetAllPostsByAuthorAsync(string authorName)
        {
            try
            {
                var query = new FindPostsByAuthor() { Author = authorName};
                var posts = await _queryDispatcher.DispatchAsync(query);
                if (posts == null || posts.Count == 0) return NoContent();
                return Ok(new PostLookupResponse
                {
                    Posts = posts,
                    Message = $"Successfully returned {posts.Count} post{(posts.Count > 1 ? "s" : string.Empty)}",
                });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "Error while processing request to retreive posts";
                _logger.LogError(ex, SAFE_ERROR_MESSAGE);
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse { Message = SAFE_ERROR_MESSAGE });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetPostByIdAsync(Guid id)
        {
            try
            {
                var query = new FindPostByIdQuery() { Id = id };
                var posts = await _queryDispatcher.DispatchAsync(query);
                if (posts == null || posts.Count == 0) return NoContent();
                return Ok(new PostLookupResponse
                {
                    Posts = posts,
                    Message = $"Successfully returned {posts.Count} post{(posts.Count > 1 ? "s" : string.Empty)}",
                });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "Error while processing request to retreive posts";
                _logger.LogError(ex, SAFE_ERROR_MESSAGE);
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse { Message = SAFE_ERROR_MESSAGE });
            }
        }


        [HttpGet("comments")]
        public async Task<ActionResult> GetAllPostsWithCommentsAsync()
        {
            try
            {
                var query = new FindPostsWithCommentsQuery();
                var posts = await _queryDispatcher.DispatchAsync(query);
                if (posts == null || posts.Count == 0) return NoContent();
                return Ok(new PostLookupResponse
                {
                    Posts = posts,
                    Message = $"Successfully returned {posts.Count} post{(posts.Count > 1 ? "s" : string.Empty)}",
                });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "Error while processing request to retreive posts";
                _logger.LogError(ex, SAFE_ERROR_MESSAGE);
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse { Message = SAFE_ERROR_MESSAGE });
            }
        }


        [HttpGet("likes/{numOfLikes}")]
        public async Task<ActionResult> GetAllPostsWithLikesAsync([FromRoute] int numOfLikes)
        {
            try
            {
                var query = new FindPostsWithLikesQuery() { NumberOfLikes = numOfLikes };
                var posts = await _queryDispatcher.DispatchAsync(query);
                if (posts == null || posts.Count == 0) return NoContent();
                return Ok(new PostLookupResponse
                {
                    Posts = posts,
                    Message = $"Successfully returned {posts.Count} post{(posts.Count > 1 ? "s" : string.Empty)}",
                });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "Error while processing request to retreive posts";
                _logger.LogError(ex, SAFE_ERROR_MESSAGE);
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse { Message = SAFE_ERROR_MESSAGE });
            }
        }
    }
}
