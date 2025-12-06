using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Api.Commands
{
    public class DeletePostCommandHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler) : ICommandHandler<DeletePostCommand>
    {
        public async Task HandleAsync(DeletePostCommand command)
        {
           var postAggregateToDelete = await eventSourcingHandler.GetByIdAsync(command.Id);
            postAggregateToDelete.DeletePost(command.Username);

            await eventSourcingHandler.SaveAsync(postAggregateToDelete);
        }
    }
}
