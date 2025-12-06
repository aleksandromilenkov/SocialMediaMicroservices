using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Api.Commands
{
    public class EditMessageCommandHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler) : ICommandHandler<EditMessageCommand>
    {
        public async Task HandleAsync(EditMessageCommand command)
        {
            var postToEdit = await eventSourcingHandler.GetByIdAsync(command.Id);
            postToEdit.EditMessage(command.Message);
            await eventSourcingHandler.SaveAsync(postToEdit);
        }
    }
}
