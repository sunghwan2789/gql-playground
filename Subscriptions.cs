using HotChocolate.Execution;
using HotChocolate.Subscriptions;

namespace GqlPlayground;

public class Subscriptions
{
    [SubscribeAndResolve]
    public ValueTask<ISourceStream<Book>> BookPublished(
        int seed,
        [Service] ITopicEventReceiver receiver)
    {
        return ValueTask.FromResult<ISourceStream<Book>>(new FetchEventStream(seed, receiver));
    }
}

public class Book
{
    public int Id { get; set; }
}

public class FetchEventStream : ISourceStream<Book>
{
    private readonly int _seed;
    private readonly ITopicEventReceiver _receiver;

    public FetchEventStream(int seed, ITopicEventReceiver receiver)
    {
        _seed = seed;
        _receiver = receiver;
    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }

    public async IAsyncEnumerable<Book> ReadEventsAsync()
    {
        foreach (var id in Enumerable.Range(1, _seed)) {
            yield return new Book { Id = id };
        }

        var stream = await _receiver.SubscribeAsync<string, Book>(nameof(Subscriptions.BookPublished));
        await foreach (var message in stream.ReadEventsAsync()) {
            yield return message;
        }
    }

    IAsyncEnumerable<object> ISourceStream.ReadEventsAsync() => ReadEventsAsync();
}
