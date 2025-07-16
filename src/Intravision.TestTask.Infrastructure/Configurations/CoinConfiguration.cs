using Intravision.TestTask.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Intravision.TestTask.Infrastructure.Configurations;

public class CoinConfiguration : IEntityTypeConfiguration<Coin>
{
    public void Configure(EntityTypeBuilder<Coin> builder)
    {
        builder.HasKey(c => c.Id);
        
        builder.OwnsOne(c => c.Denomination, denomination =>
        {
            denomination.Property(m => m.Amount)
                .HasColumnName("Denomination")
                .HasColumnType("decimal(18,2)");
                
            denomination.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .HasDefaultValue("RUB");
        });
        
        builder.Property(c => c.Quantity)
            .IsRequired();
    }
}