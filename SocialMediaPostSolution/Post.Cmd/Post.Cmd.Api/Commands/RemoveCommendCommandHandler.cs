using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Api.Commands
{
    public class RemoveCommendCommandHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler) : ICommandHandler<EditCommentCommand>
    {
        public async Task HandleAsync(EditCommentCommand command)
        {
            var postCommentToRemove = await eventSourcingHandler.GetByIdAsync(command.Id);
            postCommentToRemove.RemoveComment(command.CommentId, command.Username);
            await eventSourcingHandler.SaveAsync(postCommentToRemove);
        }
    }
}
