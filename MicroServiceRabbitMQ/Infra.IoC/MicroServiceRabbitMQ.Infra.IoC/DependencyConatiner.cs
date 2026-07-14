using Microservice.Domain.Core.Bus;
using Microservice.Infrastructure.Bus;
using Microsoft.Extensions.DependencyInjection;

namespace MicroServiceRabbitMQ.Infra.IoC;

public class DependencyContainer
{
    public static void RegisterServices(IServiceCollection services)
    {
        //Domain Bus
        services.AddTransient<IEventBus, RabbitMQBus>();
    }
}
