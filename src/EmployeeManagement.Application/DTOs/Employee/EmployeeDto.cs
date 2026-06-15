using DomainEmployee = EmployeeManagement.Domain.Entities.Employee;

namespace EmployeeManagement.Application.DTOs.Employee;

public class EmployeeDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public bool IsActive { get; set; }
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public static EmployeeDto FromEntity(DomainEmployee employee) => new()
    {
        Id = employee.Id,
        FullName = employee.FullName,
        Email = employee.Email,
        MobileNumber = employee.MobileNumber,
        JobTitle = employee.JobTitle,
        HireDate = employee.HireDate,
        IsActive = employee.IsActive,
        DepartmentId = employee.DepartmentId,
        DepartmentName = employee.Department?.Name ?? string.Empty,
        CreatedAt = employee.CreatedAt,
        UpdatedAt = employee.UpdatedAt
    };
}
