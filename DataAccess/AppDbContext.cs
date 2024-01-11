
using Microsoft.EntityFrameworkCore;

public class AppDbContext: DbContext
{
 protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=178.157.15.50;initial catalog=TestApiGizem2;user id=testapiuser;password=Er12345678;TrustServerCertificate=True");

    }

    public DbSet<Student>? Students {get; set;}
    public DbSet<Lesson>? Lessons {get; set;}

}