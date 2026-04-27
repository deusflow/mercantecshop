using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebShopMercantec.Services;
using WebShopMercantec.Shared.DTOs;

namespace WebShopMercantec.Controllers;

// admin tools for managing user credits
[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class CreditsController : ControllerBase
{
    private readonly ICreditService _creditService;

    public CreditsController(ICreditService creditService)
    {
        _creditService = creditService;
    }

    // add credits to user balance
    [HttpPost("users/{userId}/add")]
    public async Task<ActionResult<TransactionDto>> AddCredits(int userId, [FromBody] CreditAdjustmentDto dto)
    {
        var tx = await _creditService.AddCreditsAsync((uint)userId, dto.Amount, dto.Reason);
        return Ok(tx);
    }

    // get user balance
    [HttpGet("users/{userId}/balance")]
    public async Task<ActionResult> GetBalance(int userId)
    {
        var balance = await _creditService.GetBalanceAsync((uint)userId);
        return Ok(new { userId, balance });
    }

    // get user transaction history
    [HttpGet("users/{userId}/transactions")]
    public async Task<ActionResult<IEnumerable<TransactionDto>>> GetTransactions(
        int userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var txs = await _creditService.GetTransactionHistoryAsync((uint)userId, page, pageSize);
        return Ok(txs);
    }
}
