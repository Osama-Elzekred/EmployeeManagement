using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Application.Interfaces;

public interface IEmployeeRepository
{
    Task<IEnumerable<Employee>> GetAllWithDepartmentAsync();
    Task<IEnumerable<Employee>> SearchAsync(string? name, int? departmentId);
    Task<Employee?> GetByIdAsync(int id);
    Task<Employee?> GetByEmailAsync(string email);
    Task AddAsync(Employee entity);
    void Update(Employee entity);
    void Delete(Employee entity);
}
