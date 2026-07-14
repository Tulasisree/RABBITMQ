using MicroServiceRabbitMQ.Banking.Domain.Models;
using MiscroServiceRabbitMQ.Banking.Application.Models;
using MiscroServiceRabbitMQ.Banking.Application.Services;

namespace MiscroServiceRabbitMQ.Banking.Application.Interfaces;

public interface IAccountService
{
    IEnumerable<Account> GetAccounts();
    void Transfer(AccountTransfer accountTransfer);
}