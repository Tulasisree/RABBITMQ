using MicroServiceRabbitMQ.Banking.Data.Context;
using MicroServiceRabbitMQ.Banking.Domain.Interfaces;
using MicroServiceRabbitMQ.Banking.Domain.Models;

namespace MicroServiceRabbitMQ.Banking.Data.Repository;

public class AccountRepository : IAccountRepository
{
    private BankingDbContext _ctx;
    public AccountRepository(BankingDbContext ctx){
        _ctx = ctx;
    }
    public IEnumerable<Account> GetAccounts()
    {
        return _ctx.Accounts;
    }
}