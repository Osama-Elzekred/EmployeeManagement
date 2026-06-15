using DomainEmployee = EmployeeManagement.Domain.Entities.Employee;

namespace EmployeeManagement.Application.DTOs.Employee;

public class CreateEmployeeRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public bool IsActive { get; set; } = true;
    public int DepartmentId { get; set; }

    public DomainEmployee ToEntity() => new()
    {
        FullName = FullName.Trim(),
        Email = Email.Trim().ToLowerInvariant(),
        MobileNumber = MobileNumber.Trim(),
        JobTitle = JobTitle.Trim(),
        HireDate = HireDate,
        IsActive = IsActive,
        DepartmentId = DepartmentId
    };
}
