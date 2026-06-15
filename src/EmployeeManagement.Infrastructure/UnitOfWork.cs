using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Infrastructure.Data;
using EmployeeManagement.Infrastructure.Repositories;

namespace EmployeeManagement.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IEmployeeRepository? _employees;
    private IDepartmentRepository? _departments;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IEmployeeRepository Employees
        => _employees ??= new EmployeeRepository(_context);

    public IDepartmentRepository Departments
        => _departments ??= new DepartmentRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    public void Dispose()
        => _context.Dispose();
}
