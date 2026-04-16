using AccountsService.Infrastructure;
using Grpc.Core;
using AccountsService.Grpc;
using Microsoft.EntityFrameworkCore;

public class AccountsGrpcService : AccountsGrpc.AccountsGrpcBase
{
    private readonly AccountsDbContext _db;

    public AccountsGrpcService(AccountsDbContext db)
    {
        _db = db;
    }

    public override async Task<GetAccountResponse> GetAccount(GetAccountRequest request, ServerCallContext context)
    {
        var id = Guid.Parse(request.AccountId);
        var account = await _db.Accounts.FirstOrDefaultAsync(a => a.Id == id);

        if (account == null)
        {
            return new GetAccountResponse { Exists = false };
        }

        return new GetAccountResponse
        {
            Exists = true,
            AccountId = account.Id.ToString(),
            Owner = account.Owner,
            Balance = (double)account.Balance
        };
    }
}
