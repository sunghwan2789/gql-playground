using HotChocolate.Execution;
using HotChocolate.Subscriptions;

namespace GqlPlayground;

public class Subscriptions
{
    [SubscribeAndResolve]
    public ValueTask<ISourceStream<Book>> BookPublished(
        int seed,
        [Service] BookService service,
        [Service] ITopicEventReceiver receiver)
    {
        var stream = new FetchEventStream<string, Book>(service.GetBooks(seed), receiver, nameof(BookPublished));
        return ValueTask.FromResult<ISourceStream<Book>>(stream);
    }
}

public class Book
{
    public int Id { get; set; }
}

public class FetchEventStream<TTopic, TMessage> : ISourceStream<TMessage>
    where TTopic : notnull
    where TMessage : class
{
    private readonly IEnumerable<TMessage> _seed;
    private readonly ITopicEventReceiver _receiver;
    private readonly TTopic _topic;

    public FetchEventStream(IEnumerable<TMessage> seed, ITopicEventReceiver receiver, TTopic topic)
    {
        _seed = seed;
        _receiver = receiver;
        _topic = topic;
    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }

    public async IAsyncEnumerable<TMessage> ReadEventsAsync()
    {
        foreach (var book in _seed)
        {
            yield return book;
        }

        var stream = await _receiver.SubscribeAsync<TTopic, TMessage>(_topic);
        await foreach (var message in stream.ReadEventsAsync())
        {
            yield return message;
        }
    }

    IAsyncEnumerable<object> ISourceStream.ReadEventsAsync() => ReadEventsAsync();
}
