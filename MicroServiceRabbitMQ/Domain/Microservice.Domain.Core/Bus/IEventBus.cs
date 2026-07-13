using Microservice.Domain.Core.Commands;
using Microservice.Domain.Core.Events;

namespace Microservice.Domain.Core.Bus;

public interface IEventBus
{
    Task SendCommand<T>(T command) where T: Command;

    //event is a keywored in .NET to make a keyword variable we use @
    void Publish<T>(T @event) where T: Event;

    void Subscribe<T,TH>() where T: Event where TH: IEventHandler<T>;
}