using GoodHamburger.Application.Common.Interfaces;
using MediatR;

namespace GoodHamburger.Application.Common.Behaviors;

public sealed class DomainEventBehavior<TRequest, TResponse>(
    IUnitOfWork unitOfWork,
    IPublisher publisher,
    IDbContextAccessor dbContextAccessor)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var response = await next(ct);

        var entities = dbContextAccessor
            .GetTrackedEntities()
            .Where(e => e.DomainEvents.Count != 0)
            .ToList();

        var events = entities.SelectMany(e => e.DomainEvents).ToList();
        entities.ForEach(e => e.ClearDomainEvents());

        foreach (var domainEvent in events)
            await publisher.Publish(domainEvent, ct);

        return response;
    }
}
