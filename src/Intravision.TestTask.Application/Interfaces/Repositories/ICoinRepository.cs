using Intravision.TestTask.Domain.Entities;

namespace Intravision.TestTask.Application.Interfaces.Repositories;

public interface ICoinRepository
{
    Task<IEnumerable<Coin>> GetAllAsync();
    Task<Coin?> GetByDenominationAsync(decimal denomination);
    Task UpdateAsync(Coin coin);
    Task ProcessPaymentAsync(Dictionary<decimal, int> insertedCoins, Dictionary<decimal, int> changeCoins);
}