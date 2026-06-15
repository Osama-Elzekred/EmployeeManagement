namespace EmployeeManagement.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IEmployeeRepository Employees { get; }
    IDepartmentRepository Departments { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
