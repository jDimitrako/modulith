using Microsoft.EntityFrameworkCore;
using Modulith.Template.Domain.Entities;
using Modulith.Template.Domain.Interfaces;
using Modulith.Template.Infrastructure.Persistence;
using System;
using System.Threading.Tasks;

namespace Modulith.Template.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly TemplateDbContext _context;

    public UserRepository(TemplateDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
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

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Users.AnyAsync(u => u.Id == id);
    }
} 