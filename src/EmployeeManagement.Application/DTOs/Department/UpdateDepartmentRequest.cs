using DomainDepartment = EmployeeManagement.Domain.Entities.Department;

namespace EmployeeManagement.Application.DTOs.Department;

public class UpdateDepartmentRequest
{
    public string Name { get; set; } = string.Empty;

    public void ApplyTo(DomainDepartment department)
    {
        department.Name = Name.Trim();
    }
}
