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

    public async Task<bool> IsCanMakeChangeAsync(decimal amount)
    {
        var coins = await GetAllAsync();
        return IsCanMakeChangeRecursive(amount, coins.ToList());
    }

    public async Task<Dictionary<decimal, int>> CalculateChangeAsync(decimal amount)
    {
        var coins = await GetAllAsync();
        var result = new Dictionary<decimal, int>();
        var remainingAmount = amount;

        foreach (var coin in coins.OrderByDescending(c => c.Denomination.Amount))
        {
            if (remainingAmount < coin.Denomination.Amount || coin.Quantity <= 0) continue;
            var coinsNeeded = Math.Min(
                (int)(remainingAmount / coin.Denomination.Amount),
                coin.Quantity
            );

            if (coinsNeeded <= 0) continue;
            result[coin.Denomination.Amount] = coinsNeeded;
            remainingAmount -= coinsNeeded * coin.Denomination.Amount;
            remainingAmount = Math.Round(remainingAmount, 2);
        }

        if (remainingAmount > 0.01m)
            throw new InvalidOperationException("Невозможно выдать точную сдачу");

        return result;
    }

    public async Task ProcessPaymentAsync(Dictionary<decimal, int> insertedCoins, Dictionary<decimal, int> changeCoins)
    {
        // Добавляем внесенные монеты
        foreach (var (denomination, count) in insertedCoins)
        {
            var coin = await GetByDenominationAsync(denomination);
            if (coin == null) continue;
            coin.AddCoins(count);
            await UpdateAsync(coin);
        }

        // Убираем монеты для сдачи
        foreach (var (denomination, count) in changeCoins)
        {
            var coin = await GetByDenominationAsync(denomination);
            if (coin == null) continue;
            coin.RemoveCoins(count);
            await UpdateAsync(coin);
        }
    }

    private static bool IsCanMakeChangeRecursive(decimal amount, List<Coin> coins)
    {
        if (amount <= 0.01m) return true;

        return (from coin in coins.Where(c => c.Quantity > 0 && c.Denomination.Amount <= amount)
            let tempCoins = coins
                .Select(c => c.Id == coin.Id ? new Coin(c.Denomination, c.Quantity - 1) : c).ToList()
            where IsCanMakeChangeRecursive(amount - coin.Denomination.Amount, tempCoins)
            select coin).Any();
    }
}