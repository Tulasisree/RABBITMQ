using Microservice.Domain.Core.Bus;
using MicroServiceRabbitMQ.Banking.Domain.Commands;
using MicroServiceRabbitMQ.Banking.Domain.Interfaces;
using MicroServiceRabbitMQ.Banking.Domain.Models;
using MiscroServiceRabbitMQ.Banking.Application.Interfaces;
using MiscroServiceRabbitMQ.Banking.Application.Models;

namespace MiscroServiceRabbitMQ.Banking.Application.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IEventBus _bus;
    public AccountService(IAccountRepository accountRepository, IEventBus bus){
        _accountRepository = accountRepository;
        _bus = bus;
    }
    public IEnumerable<Account> GetAccounts()
    {
        return _accountRepository.GetAccounts();
    }

    public void Transfer(AccountTransfer accountTransfer)
    {
        var createTransferCommand = new CreateTransferCommand(
            accountTransfer.FromAccount,
            accountTransfer.ToAccount,
            accountTransfer.TransferAmount
        );
        _bus.SendCommand(createTransferCommand);
    }
}