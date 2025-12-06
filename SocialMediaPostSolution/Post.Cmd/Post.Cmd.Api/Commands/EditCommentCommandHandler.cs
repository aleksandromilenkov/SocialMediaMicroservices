using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Api.Commands
{
    public class EditCommentCommandHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler) : ICommandHandler<EditCommentCommand>
    {
        public async Task HandleAsync(EditCommentCommand command)
        {
            var postToEditComment = await eventSourcingHandler.GetByIdAsync(command.Id);
            postToEditComment.EditComment(command.CommentId, command.Comment, command.Username);
            await eventSourcingHandler.SaveAsync(postToEditComment);
        }
    }
}
