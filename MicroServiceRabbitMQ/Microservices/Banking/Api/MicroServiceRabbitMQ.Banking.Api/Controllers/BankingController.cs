using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MicroServiceRabbitMQ.Banking.Api.Models;
using MiscroServiceRabbitMQ.Banking.Application.Interfaces;

namespace MicroServiceRabbitMQ.Banking.Api.Controllers;

public class BankingController : Controller
{

    private readonly IAccountService _accountService;

    public BankingController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet]
    [Route("api/[controller]")]
    public IActionResult Get()
    {
        return Ok(_accountService.GetAccounts());
    }
}
