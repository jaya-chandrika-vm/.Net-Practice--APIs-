using Microsoft.EntityFrameworkCore;
using LoginService.Application.Interfaces;
using LoginService.Domain.Entities;
using LoginService.Infrastructure.Data;

namespace LoginService.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly LoginDbContext _context;

    public UserRepository(LoginDbContext context)
    {
        _context = context;
    }

    public async Task<User?> LoginAsync(string username, string password)
    {
        // For demo: plain text password check (not secure for real apps!)
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

}