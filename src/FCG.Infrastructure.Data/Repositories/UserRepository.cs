using Microsoft.EntityFrameworkCore;
using FCG.Domain.Entities;
using FCG.Application.Contracts.Repositories;
using FCG.Infrastructure.Data.Context;
using FCG.Lib.Shared.Infrastructure.Data.Repositories;

namespace FCG.Infrastructure.Data.Repositories;

public class UserRepository(ApplicationDbContext context) 
    : BaseRepository<User, ApplicationDbContext>(context), IUserRepository {

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users.AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }
}
