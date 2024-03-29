using GqlPlayground;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGraphQLServer()
    .AddQueryType<BookService>()
    .AddSubscriptionType<Subscriptions>()
    .AddInMemorySubscriptions()
    .AddTypeExtension<SubscriptionExtension>();
builder.Services.AddSingleton<BookService>();

builder.Services.AddHostedService<BookPublisher>();

var app = builder.Build();

app.UseWebSockets();

app.MapGraphQL();

app.Run();
