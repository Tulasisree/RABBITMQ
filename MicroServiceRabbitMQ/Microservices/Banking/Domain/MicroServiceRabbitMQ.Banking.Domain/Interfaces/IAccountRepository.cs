using MicroServiceRabbitMQ.Banking.Domain.Models;

namespace MicroServiceRabbitMQ.Banking.Domain.Interfaces;

public interface IAccountRepository
{
    IEnumerable<Account> GetAccounts();
}