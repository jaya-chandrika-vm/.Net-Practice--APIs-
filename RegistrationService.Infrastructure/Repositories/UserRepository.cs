using RegistrationService.Application.Interfaces;
using RegistrationService.Domain.Entities;
using RegistrationService.Infrastructure.Data;

namespace RegistrationService.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly RegistrationDbContext _context;

    public UserRepository(RegistrationDbContext context)
    {
        _context = context;
    }

    public async Task<User> RegisterAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }
}