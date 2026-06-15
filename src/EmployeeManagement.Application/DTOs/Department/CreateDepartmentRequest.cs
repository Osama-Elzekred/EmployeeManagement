using DomainDepartment = EmployeeManagement.Domain.Entities.Department;

namespace EmployeeManagement.Application.DTOs.Department;

public class CreateDepartmentRequest
{
    public string Name { get; set; } = string.Empty;

    public DomainDepartment ToEntity() => new()
    {
        Name = Name.Trim()
    };
}
