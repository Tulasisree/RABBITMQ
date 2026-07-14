
using System.Linq.Expressions;
using MediatR;
using Microservice.Domain.Core.Bus;
using Microservice.Domain.Core.Commands;
using MicroServiceRabbitMQ.Banking.Domain.Commands;
using MicroServiceRabbitMQ.Banking.Domain.Events;

namespace MicroServiceRabbitMQ.Banking.Domain.CommandHandler;

public class TransferCommandHandler : IRequestHandler<CreateTransferCommand, bool>
{
    private readonly IEventBus _bus;

    public TransferCommandHandler(IEventBus bus)
    {
        _bus = bus;
    }
    public Task<bool> Handle(CreateTransferCommand request, CancellationToken cancellationToken)
    {
        //publish event to RabbitMQ
        _bus.Publish(new TransferCreatedEvent(request.From, request.To, request.Amount));

        return Task.FromResult(true);
    }
}