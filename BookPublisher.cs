using HotChocolate.Subscriptions;

namespace GqlPlayground;

public class BookPublisher : BackgroundService
{
    private readonly ITopicEventSender _sender;

    public BookPublisher(ITopicEventSender sender)
    {
        _sender = sender;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(3));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await _sender.SendAsync(nameof(Subscriptions.BookPublished), new Book { Id = Random.Shared.Next() }, stoppingToken);
        }
    }
}
