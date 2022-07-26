using GqlPlayground;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGraphQLServer()
    .AddQueryType<Book>()
    .AddSubscriptionType<Subscriptions>()
    .AddInMemorySubscriptions();
builder.Services.AddSingleton<BookService>();

var app = builder.Build();

app.UseWebSockets();

app.MapGraphQL();

app.Run();
