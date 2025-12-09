using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Api.Commands
{
    public class RestoreReadDbCommandHandler(IEventSourcingHandler<PostAggregate> _eventSourcingHandler) : ICommandHandler<RestoreReadDbCommand>
    {
        public async Task HandleAsync(RestoreReadDbCommand command)
        {
            await _eventSourcingHandler.RepublishEventsAsync();
        }
    }
}
