using LoginService.Domain.Entities;

namespace LoginService.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> LoginAsync(string username, string password);
    Task<User?> GetUserByIdAsync(int id);
}