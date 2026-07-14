using MediatR;
using Microservice.Domain.Core.Bus;
using Microservice.Infrastructure.Bus;
using MicroServiceRabbitMQ.Banking.Data.Context;
using MicroServiceRabbitMQ.Banking.Data.Repository;
using MicroServiceRabbitMQ.Banking.Domain.CommandHandler;
using MicroServiceRabbitMQ.Banking.Domain.Commands;
using MicroServiceRabbitMQ.Banking.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using MiscroServiceRabbitMQ.Banking.Application.Interfaces;
using MiscroServiceRabbitMQ.Banking.Application.Services;

namespace MicroServiceRabbitMQ.Infra.IoC;

public class DependencyContainer
{
    public static void RegisterServices(IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(
                typeof(DependencyContainer).Assembly);
        });
        //Domain Bus
        services.AddTransient<IEventBus, RabbitMQBus>();

        //Domain Banking commands
        services.AddTransient<IRequestHandler<CreateTransferCommand, bool>,TransferCommandHandler>();

        //Application services
        services.AddTransient<IAccountService,AccountService>();

        //Data 
        services.AddTransient<IAccountRepository,AccountRepository>();
        services.AddTransient<BankingDbContext>();
    }
}
