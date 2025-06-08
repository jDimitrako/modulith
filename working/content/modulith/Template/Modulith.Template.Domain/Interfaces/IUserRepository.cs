using Modulith.Template.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Modulith.Template.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task<bool> ExistsAsync(Guid id);
} 