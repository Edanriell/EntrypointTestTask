using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Server.Domain.Users;

namespace Server.Infrastructure.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.FirstName)
            .HasMaxLength(200)
            .HasConversion(
                firstName => firstName.Value,
                value => new FirstName(value)
            );

        builder.Property(user => user.LastName)
            .HasMaxLength(200)
            .HasConversion(
                firstName => firstName.Value,
                value => new LastName(value)
            );

        builder.Property(user => user.Email)
            .HasMaxLength(400)
            .HasConversion(
                email => email.Value.ToLowerInvariant(),
                value => new Email(value)
            );

        builder.Property(user => user.PhoneNumber)
            .HasMaxLength(50)
            .IsRequired()
            .HasConversion(
                phoneNumber => phoneNumber.Value,
                value => new PhoneNumber(value)
            );

        builder.Property(user => user.Gender)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.OwnsOne(user => user.Address, addressBuilder =>
        {
            addressBuilder.Property(a => a.Street)
                .HasColumnName("address_street")
                .HasMaxLength(200)
                .IsRequired();

            addressBuilder.Property(a => a.City)
                .HasColumnName("address_city")
                .HasMaxLength(100)
                .IsRequired();

            addressBuilder.Property(a => a.Country)
                .HasColumnName("address_country")
                .HasMaxLength(100)
                .IsRequired();

            addressBuilder.Property(a => a.ZipCode)
                .HasColumnName("address_zipcode")
                .HasMaxLength(20)
                .IsRequired();
        });

        builder.Property(user => user.CreatedAt)
            .IsRequired();

        builder.HasIndex(user => user.Email).IsUnique();
        builder.HasIndex(user => user.IdentityId).IsUnique();
        builder.HasIndex(user => user.PhoneNumber).IsUnique();
        builder.HasIndex(user => user.CreatedAt)
            .HasDatabaseName("ix_users_created_at");
        builder.HasIndex(user => user.FirstName)
            .HasDatabaseName("ix_users_first_name");
        builder.HasIndex(user => user.LastName)
            .HasDatabaseName("ix_users_last_name");
        builder.HasIndex(user => new { user.CreatedAt, user.FirstName })
            .HasDatabaseName("ix_users_created_at_first_name");
        builder.HasIndex(user => new { user.CreatedAt, user.LastName })
            .HasDatabaseName("ix_users_created_at_last_name");
        builder.HasIndex(user => new { user.CreatedAt, user.Email })
            .HasDatabaseName("ix_users_created_at_email");
        builder.HasIndex(user => new { user.FirstName, user.LastName })
            .HasDatabaseName("ix_users_first_name_last_name");
    }
}
