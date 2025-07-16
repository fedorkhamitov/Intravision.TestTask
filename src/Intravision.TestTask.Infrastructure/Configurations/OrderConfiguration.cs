using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Intravision.TestTask.Domain.Entities;

namespace Intravision.TestTask.Infrastructure.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);
        
        builder.Property(o => o.OrderDate)
            .IsRequired();
            
        builder.OwnsOne(o => o.TotalAmount, totalAmount =>
        {
            totalAmount.Property(m => m.Amount)
                .HasColumnName("TotalAmount")
                .HasColumnType("decimal(18,2)");
                
            totalAmount.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .HasDefaultValue("RUB");
        });
        
        builder.OwnsMany(o => o.Items, item =>
        {
            item.Property(i => i.ProductId).IsRequired();
            item.Property(i => i.BrandId).IsRequired();
            item.Property(i => i.Quantity).IsRequired();
            
            item.OwnsOne(i => i.TotalPrice, totalPrice =>
            {
                totalPrice.Property(m => m.Amount)
                    .HasColumnName("TotalPrice")
                    .HasColumnType("decimal(18,2)");
                    
                totalPrice.Property(m => m.Currency)
                    .HasColumnName("Currency")
                    .HasMaxLength(3)
                    .HasDefaultValue("RUB");
            });
        });
    }
}