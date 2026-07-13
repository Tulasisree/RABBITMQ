using Microservice.Domain.Core.Events;

namespace Microservice.Domain.Core.Bus;

public interface IEventHandler<in TEvent> : IEventHandler where TEvent : Event
{
    Task Handle(TEvent @event);
}

public interface IEventHandler
{
    
}