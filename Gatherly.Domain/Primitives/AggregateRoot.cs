namespace Gatherly.Domain.Primitives;

public abstract class AggregateRoot : Entity
{
    private readonly List<IDomainEvent> _events = new();
    protected AggregateRoot(Guid id) 
        : base(id)
    {
    }

    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _events.Add(domainEvent);
    }
}
