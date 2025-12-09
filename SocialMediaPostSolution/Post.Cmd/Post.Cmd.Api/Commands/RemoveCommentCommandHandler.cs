using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Api.Commands
{
    public class RemoveCommentCommandHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler) : ICommandHandler<RemoveCommentCommand>
    {
        public async Task HandleAsync(RemoveCommentCommand command)
        {
            var postCommentToRemove = await eventSourcingHandler.GetByIdAsync(command.Id);
            postCommentToRemove.RemoveComment(command.CommentId, command.Username);
            await eventSourcingHandler.SaveAsync(postCommentToRemove);
        }
    }
}
