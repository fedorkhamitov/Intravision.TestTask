using Intravision.TestTask.Domain.Exceptions;
using Intravision.TestTask.Domain.Shared;

namespace Intravision.TestTask.Domain.Entities;

public class Brand : Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    
    private Brand() { } // Для EF Core
    
    public Brand(string name, string description)
    {
        Id = Guid.NewGuid();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
    }
    
    public void UpdateInfo(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Название бренда не может быть пустым");

        Name = name;
        Description = description ?? string.Empty;
    }
}