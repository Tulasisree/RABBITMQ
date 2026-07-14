

using Microservice.Domain.Core.Commands;

namespace MicroServiceRabbitMQ.Banking.Domain.Commands;

public abstract class TransferCommand : Command
{
    public int From {get; protected set;}
    public int To {get; protected set;}
    public decimal Amount {get; protected set;}
}