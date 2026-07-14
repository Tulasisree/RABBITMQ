using System.Linq;
using System.Text;
using MediatR;
using Microservice.Domain.Core.Bus;
using Microservice.Domain.Core.Commands;
using Microservice.Domain.Core.Events;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Microservice.Infrastructure.Bus;


public sealed class RabbitMQBus : IEventBus
{
    private readonly IMediator _mediator;
    private readonly Dictionary<string,List<Type>> _handlers;//to store all handlers and eventtypes, unique handlers whenever someone subscibes to an event using required handler
    private readonly List<Type> _eventTypes;

    public RabbitMQBus(IMediator mediator)
    {
        _mediator = mediator;
        _handlers = new Dictionary<string, List<Type>>();
        _eventTypes = new List<Type>();
    }
    public async void Publish<T>(T @event) where T : Event
    {
        var factory = new ConnectionFactory() {HostName = "localhost"};
        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();
        var eventName = @event.GetType().Name;
        await channel.QueueDeclareAsync(eventName, true, false, false, null);

        var message = JsonConvert.SerializeObject(@event);
        var body = Encoding.UTF8.GetBytes(message);

        await channel.BasicPublishAsync("",eventName,body);
    }

    public Task SendCommand<T>(T command) where T : Command
    {
        return _mediator.Send(command);
    }

    public void Subscribe<T, TH>()
        where T : Event
        where TH : IEventHandler<T>
    {
        var eventName = typeof(T).Name;
        var handlerType = typeof(TH);

        if(!_eventTypes.Contains(typeof(T)))
        {
            _eventTypes.Add(typeof(T));
        }

        if(!_handlers.ContainsKey(eventName))
        {
            _handlers.Add(eventName,new List<Type>());
        }

        if(_handlers[eventName].Any(s=> s.GetType() == handlerType))
        {
            throw new ArgumentException($"Handler Type {handlerType.Name} already registered for ${eventName}.", nameof(handlerType));
        }

        _handlers[eventName].Add(handlerType);

        StartBasicConsume<T>();
    }

    private async void StartBasicConsume<T>() where T : Event
    {
        var factory = new ConnectionFactory(){ HostName = "localhost"};
        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        //event thats coming in
        var eventName = typeof(T).Name;

        await channel.QueueDeclareAsync(eventName,true,false,false,null);

        var consumer = new AsyncEventingBasicConsumer(channel);

        //delegate -> placeholder for an event
        consumer.ReceivedAsync += Consumer_Received;

        await channel.BasicConsumeAsync(eventName,true,consumer);
    }

    private async Task Consumer_Received(object sender, BasicDeliverEventArgs e)
    {
        var eventName = e.RoutingKey;
        var message = Encoding.UTF8.GetString(e.Body.ToArray());

        try
        {
            await ProcessEvent(eventName, message).ConfigureAwait(false);
        }
        catch (Exception ex)
        {

        }
    }

    private async Task ProcessEvent(string eventName, string message)
    {
        if(_handlers.ContainsKey(eventName))
        {
            var subscriptions = _handlers[eventName];
            foreach ( var subscription in subscriptions)
            {
                //dynamic approches to generics
                var handler = Activator.CreateInstance(subscription);
                if (handler == null) continue;
                var eventType = _eventTypes.SingleOrDefault(t => t.Name == eventName);
                var @event = JsonConvert.DeserializeObject(message, eventType);
                var concreteType = typeof(IEventHandler<>).MakeGenericType(eventType);

                await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] {@event});
            }
        }
    }
}