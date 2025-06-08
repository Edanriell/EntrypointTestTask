using Domain.Entities;

namespace Infrastructure.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
	public void Configure(EntityTypeBuilder<Payment> builder)
	{
		builder.ToTable("Payments");
		builder.HasKey(p => p.Id);

		builder.Property(p => p.Id).HasColumnName(nameof(Payment.Id)).IsRequired();
		builder.Property(p => p.Amount).HasColumnName(nameof(Payment.Amount)).HasColumnType("decimal(18, 2)")
		   .IsRequired();
		builder.Property(p => p.PaymentDate).HasColumnName(nameof(Payment.PaymentDate)).IsRequired();
		builder.Property(p => p.PaymentMethod).HasColumnName(nameof(Payment.PaymentMethod)).IsRequired();
		builder.Property(p => p.TransactionId).HasColumnName(nameof(Payment.TransactionId)).HasMaxLength(50)
		   .IsRequired();
		builder.Property(p => p.OrderId).HasColumnName(nameof(Payment.OrderId));
	}
}