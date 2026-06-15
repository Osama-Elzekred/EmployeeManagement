using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Repositories;

public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Employee>> GetAllWithDepartmentAsync()
        => await _dbSet
            .AsNoTracking()
            .Include(e => e.Department)
            .OrderBy(e => e.FullName)
            .ToListAsync();

    public async Task<IEnumerable<Employee>> SearchAsync(string? name, int? departmentId)
    {
        var query = _dbSet.AsNoTracking().Include(e => e.Department).AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(e => e.FullName.Contains(name) || e.Department.Name.Contains(name));

        if (departmentId.HasValue && departmentId > 0)
            query = query.Where(e => e.DepartmentId == departmentId.Value);

        return await query.OrderBy(e => e.FullName).ToListAsync();
    }

    public async Task<Employee?> GetByEmailAsync(string email)
        => await _dbSet.FirstOrDefaultAsync(e => e.Email == email);

    public override async Task<Employee?> GetByIdAsync(int id)
        => await _dbSet
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.Id == id);
}
