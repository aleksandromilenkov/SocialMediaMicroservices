using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Api.Commands
{
    public class AddCommentCommandHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler) : ICommandHandler<AddCommentCommand>
    {
        public async Task HandleAsync(AddCommentCommand command)
        {
            var postToAddComment = await eventSourcingHandler.GetByIdAsync(command.Id);
            postToAddComment.AddComment(command.Comment, command.Username);
            await eventSourcingHandler.SaveAsync(postToAddComment);
        }
    }
}
