using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Infrastructure;
using CQRS.Core.Queries;
using Microsoft.EntityFrameworkCore;
using Post.Query.Api.Queries;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.Consumers;
using Post.Query.Infrastructure.DataAccess;
using Post.Query.Infrastructure.Dispatchers;
using Post.Query.Infrastructure.Handlers;
using Post.Query.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextFactory<DatabaseContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IQueryHandler, QueryHandler>();
builder.Services.AddScoped<IEventHandler, Post.Query.Infrastructure.Handlers.EventHandler>();
builder.Services.Configure<ConsumerConfig>(builder.Configuration.GetSection("ConsumerConfig"));
builder.Services.AddScoped<IEventConsumer, EventConsumer>();
builder.Services.AddHostedService<ConsumerHostedService>();
builder.Services.AddSingleton<IQueryDispatcher<PostEntity>, QueryDispatcher>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var provider = scope.ServiceProvider;

    var dispatcher = provider.GetRequiredService<IQueryDispatcher<PostEntity>>();
    var queryHandler = provider.GetRequiredService<IQueryHandler>();

    dispatcher.RegisterHandler<FindAllPostsQuery>(queryHandler.HandleAsync);
    dispatcher.RegisterHandler<FindPostByIdQuery>(queryHandler.HandleAsync);
    dispatcher.RegisterHandler<FindPostsByAuthor>(queryHandler.HandleAsync);
    dispatcher.RegisterHandler<FindPostsWithCommentsQuery>(queryHandler.HandleAsync);
    dispatcher.RegisterHandler<FindPostsWithLikesQuery>(queryHandler.HandleAsync);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseHttpsRedirection();

app.Run();
