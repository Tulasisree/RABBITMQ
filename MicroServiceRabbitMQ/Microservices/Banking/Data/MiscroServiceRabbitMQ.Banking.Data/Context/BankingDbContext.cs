using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MicroServiceRabbitMQ.Banking.Domain.Models;
namespace MicroServiceRabbitMQ.Banking.Data.Context;

public class BankingDbContext : DbContext
{
    public BankingDbContext(DbContextOptions options) : base(options)
    {}

    public DbSet<Account> Accounts { get; set; }
}