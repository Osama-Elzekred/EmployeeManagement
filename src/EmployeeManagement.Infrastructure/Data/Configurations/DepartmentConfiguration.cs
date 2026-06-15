using EmployeeManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeManagement.Infrastructure.Data.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(d => d.Name)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        // Seed data — 5 default departments
        builder.HasData(
            new Department { Id = 1, Name = "Human Resources",   CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
            new Department { Id = 2, Name = "Engineering",        CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
            new Department { Id = 3, Name = "Marketing",          CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
            new Department { Id = 4, Name = "Finance",            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
            new Department { Id = 5, Name = "Operations",         CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false }
        );
    }
}
