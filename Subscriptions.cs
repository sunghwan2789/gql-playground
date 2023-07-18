using HotChocolate.Execution;
using HotChocolate.Subscriptions;

namespace GqlPlayground;

public class Subscriptions
{
    [Subscribe(With = nameof(BookPublishedStream))]
    public Book BookPublished([EventMessage] Book book) => book;

    public ValueTask<IAsyncEnumerable<Book>> BookPublishedStream(
        int seed,
        [Service] BookService service,
        [Service] ITopicEventReceiver receiver)
    {
        var stream = new FetchEventStream<Book>(service.GetBooks(seed), receiver, nameof(BookPublished));
        return ValueTask.FromResult<IAsyncEnumerable<Book>>(stream);
    }
}

public class Book
{
    public int Id { get; set; }
}

public class FetchEventStream<TMessage> : IAsyncEnumerable<TMessage>
    where TMessage : class
{
    private readonly IEnumerable<TMessage> _seed;
    private readonly ITopicEventReceiver _receiver;
    private readonly string _topic;

    public FetchEventStream(IEnumerable<TMessage> seed, ITopicEventReceiver receiver, string topic)
    {
        _seed = seed;
        _receiver = receiver;
        _topic = topic;
    }

    public async IAsyncEnumerator<TMessage> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        foreach (var book in _seed)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            yield return book;
        }

        await using var stream = await _receiver.SubscribeAsync<TMessage>(_topic, cancellationToken);
        await foreach (var message in stream.ReadEventsAsync())
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            yield return message;
        }
    }
}
