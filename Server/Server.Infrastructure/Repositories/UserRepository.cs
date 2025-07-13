using Bookify.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Server.Domain.Users;

namespace Server.Infrastructure.Repositories;

internal sealed class UserRepository
    : Repository<User>,
        IUserRepository
{
    public UserRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<User>()
            .Include(u => u.Roles)
            .Include(u => u.Orders)
            .ToListAsync(cancellationToken);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<User>()
            .Include(u => u.Roles)
            .Include(u => u.Orders)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByPhoneNumberAsync(
        PhoneNumber phoneNumber, CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<User>()
            .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);
    }

    public override void Add(User user)
    {
        foreach (Role role in user.Roles)
        {
            DbContext.Attach(role);
        }

        DbContext.Add(
            user
        );
    }

    public void Update(User user)
    {
        // Attach roles to prevent EF from trying to insert them
        foreach (Role role in user.Roles)
        {
            EntityEntry<Role> trackedRole = DbContext.Entry(role);
            if (trackedRole.State == EntityState.Detached)
            {
                DbContext.Attach(role);
            }
        }

        DbContext.Update(user);
    }

    public void Remove(User user) { DbContext.Remove(user); }

    public async Task<User?> GetByEmailAsync(
        Email email, CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<User>()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }
}
