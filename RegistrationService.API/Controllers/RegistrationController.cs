using Microsoft.AspNetCore.Mvc;
using RegistrationService.Application.Interfaces;
using RegistrationService.Domain.Entities;

namespace RegistrationService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegistrationController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public RegistrationController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] User user)
    {
        var createdUser = await _userRepository.RegisterAsync(user);
        return CreatedAtAction(nameof(Register), new { id = createdUser.Id }, createdUser);
    }
}