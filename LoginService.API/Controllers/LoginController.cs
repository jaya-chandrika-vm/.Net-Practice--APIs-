using LoginService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LoginService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<LoginController> _logger;
    private readonly HttpClient _httpClient;

    private static readonly ActivitySource ActivitySource =
        new("login-service"); //  This line defines a custom span source

    public LoginController(
        IUserRepository userRepository,
        ILogger<LoginController> logger,
        IHttpClientFactory httpClientFactory)
    {
        _userRepository = userRepository;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
    }

    //  LOGIN — service span + DB span + logs
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        using var activity = ActivitySource.StartActivity("Login Process"); //Creates a custom span Appears as a child span in the trace

        _logger.LogInformation("Login start for {Username}", request.Username);

        try
        {
            var user = await _userRepository.LoginAsync(
                request.Username,
                request.Password);

            if (user == null)
            {
                activity?.SetTag("login.status", "failed");
                _logger.LogWarning("Login failed for {Username}", request.Username);
                return Unauthorized("Invalid credentials");
            }

            activity?.SetTag("login.status", "success");
            _logger.LogInformation("Login successful for {Username}", request.Username);

            return Ok("Login successful");
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error);
            activity?.SetTag("exception.type", ex.GetType().Name);
            activity?.SetTag("exception.message", ex.Message);

            _logger.LogError(ex, "Login exception");
            throw;
        }
    }

    //  DB span example
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        using var activity = ActivitySource.StartActivity("GetUserById");

        _logger.LogInformation("Fetching user {UserId}", id);

        var user = await _userRepository.GetUserByIdAsync(id);
        return user == null ? NotFound() : Ok(user);
    }

    //  Downstream/external API span
    [HttpGet("health-check")]
    public async Task<IActionResult> ExternalApi()
    {
        using var activity = ActivitySource.StartActivity("External API Call");

        var response = await _httpClient.GetAsync(
            "https://jsonplaceholder.typicode.com/posts/1");  // HttpClient instrumentation creates downstream span

        activity?.SetTag("external.status", (int)response.StatusCode);
        _logger.LogInformation("External API status {Status}", response.StatusCode);

        return Ok(response.StatusCode);
    }

    // Exception telemetry
    [HttpGet("test-exception")]
    public IActionResult TestException()
    {
        using var activity = ActivitySource.StartActivity("Test Exception");

        _logger.LogWarning("Test exception invoked");
        throw new InvalidOperationException("Telemetry test exception");
    }

    public record LoginRequest(string Username, string Password);
}