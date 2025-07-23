using Intravision.TestTask.Domain.Entities;

namespace Intravision.TestTask.Application.Services;

public class ChangeCalculator
{
    public bool IsCanMakeChange(IEnumerable<Coin> coins, decimal amount)
    {
        try
        {
            _ = CalculateChange(coins, amount);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public Dictionary<decimal, int> CalculateChange(IEnumerable<Coin> coins, decimal amount)
    {
        var sortedCoins = coins
            .Where(c => c.Denomination.Amount <= amount && c.Quantity > 0)
            .OrderByDescending(c => c.Denomination.Amount)
            .ToList();

        var result = new Dictionary<decimal, int>();
        var remaining = amount;

        foreach (var coin in sortedCoins)
        {
            var denom = coin.Denomination.Amount;
            var maxFit = (int)(remaining / denom);
            var count = Math.Min(maxFit, coin.Quantity);

            if (count <= 0) continue;
            result[denom] = count;
            remaining -= denom * count;
            remaining = Math.Round(remaining, 2);
            if (remaining == 0) break;
        }

        if (remaining > 0)
            throw new InvalidOperationException("Невозможно подобрать сдачу");

        return result;
    }
}