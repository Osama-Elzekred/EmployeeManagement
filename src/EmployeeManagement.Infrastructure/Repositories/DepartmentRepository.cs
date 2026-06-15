using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Repositories;

public class DepartmentRepository : BaseRepository<Department>, IDepartmentRepository
{
    public DepartmentRepository(AppDbContext context) : base(context)
    {
    }

    public override async Task<IEnumerable<Department>> GetAllAsync()
        => await _dbSet
            .AsNoTracking()
            .Include(d => d.Employees)
            .OrderBy(d => d.Name)
            .ToListAsync();

    public override async Task<Department?> GetByIdAsync(int id)
        => await _dbSet
            .Include(d => d.Employees)
            .FirstOrDefaultAsync(d => d.Id == id);

    public async Task<Department?> GetByNameAsync(string name)
        => await _dbSet.FirstOrDefaultAsync(d => d.Name == name);
}
