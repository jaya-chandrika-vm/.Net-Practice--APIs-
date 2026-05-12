using Microsoft.EntityFrameworkCore;
using RegistrationService.Domain.Entities;

namespace RegistrationService.Infrastructure.Data;

public class RegistrationDbContext : DbContext
{
    public RegistrationDbContext(DbContextOptions<RegistrationDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
}