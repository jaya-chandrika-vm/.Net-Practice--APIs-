using RegistrationService.Domain.Entities;

namespace RegistrationService.Application.Interfaces;

public interface IUserRepository
{
    Task<User> RegisterAsync(User user);
}