using HotChocolate.Resolvers;

namespace GqlPlayground;

public class SubscriptionExtension(
    ILogger<SubscriptionExtension> logger
)
    : ObjectTypeExtension<Subscriptions>
{
    protected override void Configure(IObjectTypeDescriptor<Subscriptions> descriptor)
    {
        descriptor.Field(f => f.BookPublished(default, default!, default!))
            .Use(next => async context =>
            {
                logger.LogInformation("BookPublished {@Variables}", context);
                await next(context);
            })
            ;
    }
}
