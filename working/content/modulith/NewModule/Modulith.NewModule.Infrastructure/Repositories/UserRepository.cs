using Microsoft.EntityFrameworkCore;
using Modulith.NewModule.Domain.Entities;
using Modulith.NewModule.Domain.Interfaces;
using Modulith.NewModule.Infrastructure.Data;

namespace Modulith.NewModule.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly NewModuleDbContext _context;

    public UserRepository(NewModuleDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Users.AnyAsync(u => u.Id == id);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(User user)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }
} 