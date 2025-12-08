using Confluent.Kafka;
using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using CQRS.Core.Repositories;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using Post.Cmd.Api.Commands;
using Post.Cmd.Domain.Aggregates;
using Post.Cmd.Infrastructure.Config;
using Post.Cmd.Infrastructure.Dispatchers;
using Post.Cmd.Infrastructure.Handlers;
using Post.Cmd.Infrastructure.Producers;
using Post.Cmd.Infrastructure.Stores;
using CQRS.Core.Events;
using Post.Common.Events;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

BsonClassMap.RegisterClassMap<BaseEvent>();
BsonClassMap.RegisterClassMap<PostCreatedEvent>();
BsonClassMap.RegisterClassMap<MessageUpdatedEvent>();
BsonClassMap.RegisterClassMap<PostRemovedEvent>();
BsonClassMap.RegisterClassMap<PostLikedEvent>();
BsonClassMap.RegisterClassMap<CommentAddedEvent>();
BsonClassMap.RegisterClassMap<CommentUpdatedEvent>();
BsonClassMap.RegisterClassMap<CommentRemovedEvent>();


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


builder.Services.Configure<MongoDbConfig>(builder.Configuration.GetSection(nameof(MongoDbConfig)));
builder.Services.Configure<ProducerConfig>(builder.Configuration.GetSection(nameof(ProducerConfig)));
builder.Services.AddScoped<IEventStoreRepository, EventStoreRepository>();
builder.Services.AddScoped<IEventProducer, EventProducer>();
builder.Services.AddScoped<IEventStore, EventStore>();
builder.Services.AddScoped<ICommandDispatcher, CommandDispatcher>();
builder.Services.AddScoped<IEventSourcingHandler<PostAggregate>, EventSourcingHandler>();
builder.Services.AddScoped<ICommandHandler<NewPostCommand>, NewPostCommandHandler>();
builder.Services.AddScoped<ICommandHandler<AddCommentCommand>, AddCommentCommandHandler>();
builder.Services.AddScoped<ICommandHandler<EditMessageCommand>, EditMessageCommandHandler>();
builder.Services.AddScoped<ICommandHandler<LikePostCommand>, LikePostCommandHandler>();
builder.Services.AddScoped<ICommandHandler<DeletePostCommand>, DeletePostCommandHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();

