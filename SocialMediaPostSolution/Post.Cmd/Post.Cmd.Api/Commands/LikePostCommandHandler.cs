using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Api.Commands
{
    public class LikePostCommandHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler) : ICommandHandler<LikePostCommand>
    {
        public async Task HandleAsync(LikePostCommand command)
        {
            var postToLike = await eventSourcingHandler.GetByIdAsync(command.Id);
            postToLike.LikePost();
            await eventSourcingHandler.SaveAsync(postToLike);
        }
    }
}
