using MicroServiceRabbitMQ.Banking.Domain.Interfaces;
using MicroServiceRabbitMQ.Banking.Domain.Models;
using MiscroServiceRabbitMQ.Banking.Application.Interfaces;

namespace MiscroServiceRabbitMQ.Banking.Application.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    public AccountService(IAccountRepository accountRepository){
        _accountRepository = accountRepository;
    }
    public IEnumerable<Account> GetAccounts()
    {
        return _accountRepository.GetAccounts();
    }
}