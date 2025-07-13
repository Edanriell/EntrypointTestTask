using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Server.Application.Abstractions.Clock;
using Server.Application.Exceptions;
using Server.Domain.Abstractions;

namespace Server.Infrastructure;

public sealed class ApplicationDbContext
    : DbContext,
        IUnitOfWork
{
    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };

    private readonly IDateTimeProvider _dateTimeProvider;

    public ApplicationDbContext(
        DbContextOptions options,
        IDateTimeProvider dateTimeProvider)
        : base(
            options
        )
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken
            = default)
    {
        try
        {
            int result = await base.SaveChangesAsync(
                cancellationToken
            );

            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyException(
                "Concurrency exception occurred.",
                ex
            );
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly
        );

        base.OnModelCreating(
            modelBuilder
        );
    }
}
