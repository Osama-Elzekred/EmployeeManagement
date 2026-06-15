using DomainDepartment = EmployeeManagement.Domain.Entities.Department;

namespace EmployeeManagement.Application.DTOs.Department;

public class DepartmentDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int EmployeeCount { get; set; }
    public DateTime CreatedAt { get; set; }

    public static DepartmentDto FromEntity(DomainDepartment department) => new()
    {
        Id = department.Id,
        Name = department.Name,
        EmployeeCount = department.Employees?.Count(e => !e.IsDeleted) ?? 0,
        CreatedAt = department.CreatedAt
    };
}
