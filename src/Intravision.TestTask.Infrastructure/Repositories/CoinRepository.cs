using Intravision.TestTask.Application.Interfaces.Repositories;
using Intravision.TestTask.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Intravision.TestTask.Infrastructure.Repositories;

public class CoinRepository : ICoinRepository
{
    private readonly ApplicationDbContext _context;

    public CoinRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Coin>> GetAllAsync()
    {
        return await _context.Coins.OrderByDescending(c => c.Denomination.Amount).ToListAsync();
    }

    public async Task<Coin?> GetByDenominationAsync(decimal denomination)
    {
        return await _context.Coins
            .FirstOrDefaultAsync(c => c.Denomination.Amount == denomination);
    }

    public async Task UpdateAsync(Coin coin)
    {
        _context.Coins.Update(coin);
        await _context.SaveChangesAsync();
    }

    public async Task ProcessPaymentAsync(Dictionary<decimal, int> insertedCoins, Dictionary<decimal, int> changeCoins)
    {
        foreach (var (denomination, count) in insertedCoins)
        {
            var coin = await GetByDenominationAsync(denomination);
            if (coin == null) continue;
            coin.AddCoins(count);
            await UpdateAsync(coin);
        }

        foreach (var (denomination, count) in changeCoins)
        {
            var coin = await GetByDenominationAsync(denomination);
            if (coin == null) continue;
            coin.RemoveCoins(count);
            await UpdateAsync(coin);
        }
    }
}