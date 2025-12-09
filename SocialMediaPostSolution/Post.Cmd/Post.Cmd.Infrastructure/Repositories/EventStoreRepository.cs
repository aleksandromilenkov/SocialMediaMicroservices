using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQRS.Core.Domain;
using CQRS.Core.Events;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using Post.Cmd.Infrastructure.Config;

namespace CQRS.Core.Repositories
{
    public class EventStoreRepository : IEventStoreRepository
    {
        private readonly IMongoCollection<EventModel> _eventStoreCollection;
        public EventStoreRepository(IOptions<MongoDbConfig> config)
        {
            var mongoClient = new MongoClient(config.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(config.Value.Database);
            _eventStoreCollection = mongoDatabase.GetCollection<EventModel>(config.Value.Collection);
        }
        public async Task CreateAsync(EventModel @event)
        {
            await _eventStoreCollection.InsertOneAsync(@event).ConfigureAwait(false);
        }

        public async Task<List<EventModel>> FindAllAsync()
        {
            return await _eventStoreCollection.Find(_ => true).ToListAsync().ConfigureAwait(false);
        }

        public async Task<List<EventModel>> FindByAggregateId(Guid aggregateId)
        {
            return await _eventStoreCollection.Find(e => e.AggregateIdentifier == aggregateId)
                .ToListAsync()
                .ConfigureAwait(false);
        }
    }
}
