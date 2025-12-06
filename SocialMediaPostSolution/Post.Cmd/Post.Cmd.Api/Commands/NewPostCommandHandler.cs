using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Api.Commands
{
    public class NewPostCommandHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler) : ICommandHandler<NewPostCommand>
    {
        public async Task HandleAsync(NewPostCommand command)
        {
            var aggregate = new PostAggregate(command.Id, command.Author, command.Message);
            await eventSourcingHandler.SaveAsync(aggregate);
        }
    }
}
