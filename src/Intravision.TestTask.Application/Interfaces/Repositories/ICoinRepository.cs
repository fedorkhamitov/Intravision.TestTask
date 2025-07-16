using Intravision.TestTask.Domain.Entities;

namespace Intravision.TestTask.Application.Interfaces;

public interface ICoinRepository
{
    Task<IEnumerable<Coin>> GetAllAsync();
    Task<Coin?> GetByDenominationAsync(decimal denomination);
    Task UpdateAsync(Coin coin);
    Task<bool> CanMakeChangeAsync(decimal amount);
    Task<Dictionary<decimal, int>> CalculateChangeAsync(decimal amount);
    Task ProcessPaymentAsync(Dictionary<decimal, int> insertedCoins, Dictionary<decimal, int> changeCoins);
}