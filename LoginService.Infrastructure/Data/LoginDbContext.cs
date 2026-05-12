using Microsoft.EntityFrameworkCore;
using LoginService.Domain.Entities;

namespace LoginService.Infrastructure.Data;

public class LoginDbContext : DbContext
{
    public LoginDbContext(DbContextOptions<LoginDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
}