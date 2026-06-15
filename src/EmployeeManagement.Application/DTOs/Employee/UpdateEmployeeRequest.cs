using DomainEmployee = EmployeeManagement.Domain.Entities.Employee;

namespace EmployeeManagement.Application.DTOs.Employee;

public class UpdateEmployeeRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public bool IsActive { get; set; }
    public int DepartmentId { get; set; }

    public void ApplyTo(DomainEmployee employee)
    {
        employee.FullName = FullName.Trim();
        employee.Email = Email.Trim().ToLowerInvariant();
        employee.MobileNumber = MobileNumber.Trim();
        employee.JobTitle = JobTitle.Trim();
        employee.HireDate = HireDate;
        employee.IsActive = IsActive;
        employee.DepartmentId = DepartmentId;
    }
}
