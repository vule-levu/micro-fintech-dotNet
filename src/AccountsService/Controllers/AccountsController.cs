using Microsoft.AspNetCore.Mvc;
using AccountsService.Infrastructure;
using AccountsService.Domain.Entities;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly AccountsDbContext _db;
    public AccountsController(AccountsDbContext db) => _db = db;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var a = await _db.Accounts.FindAsync(id);
        if (a == null) return NotFound();
        return Ok(a);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAccountDto dto)
    {
        var a = new Account { Id = Guid.NewGuid(), Owner = dto.Owner, Balance = dto.InitialBalance };
        _db.Accounts.Add(a);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = a.Id }, a);
    }
}

public record CreateAccountDto(string Owner, decimal InitialBalance);
