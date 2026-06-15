using EmployeeManagement.Application.Common;
using EmployeeManagement.Application.DTOs.Employee;
using EmployeeManagement.Application.Interfaces;
using FluentValidation;

namespace EmployeeManagement.Application.Services;

public class EmployeeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateEmployeeRequest> _createValidator;
    private readonly IValidator<UpdateEmployeeRequest> _updateValidator;

    public EmployeeService(
        IUnitOfWork unitOfWork,
        IValidator<CreateEmployeeRequest> createValidator,
        IValidator<UpdateEmployeeRequest> updateValidator)
    {
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<Result<IEnumerable<EmployeeDto>>> GetAllAsync(string? name, int? departmentId)
    {
        IEnumerable<Domain.Entities.Employee> employees;

        if (!string.IsNullOrWhiteSpace(name) || departmentId.HasValue)
            employees = await _unitOfWork.Employees.SearchAsync(name, departmentId);
        else
            employees = await _unitOfWork.Employees.GetAllWithDepartmentAsync();

        var dtos = employees.Select(EmployeeDto.FromEntity);
        return Result<IEnumerable<EmployeeDto>>.Success(dtos);
    }

    public async Task<Result<EmployeeDto>> GetByIdAsync(int id)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(id);
        if (employee is null)
            return Result<EmployeeDto>.Failure($"Employee with ID {id} was not found.", 404);

        return Result<EmployeeDto>.Success(EmployeeDto.FromEntity(employee));
    }

    public async Task<Result<EmployeeDto>> CreateAsync(CreateEmployeeRequest request)
    {
        var validation = await _createValidator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            var errors = validation.Errors.Select(e => e.ErrorMessage);
            return Result<EmployeeDto>.Failure(errors, 400);
        }

        var department = await _unitOfWork.Departments.GetByIdAsync(request.DepartmentId);
        if (department is null)
            return Result<EmployeeDto>.Failure("Selected department does not exist.", 400);

        var existingEmail = await _unitOfWork.Employees.GetByEmailAsync(request.Email.Trim().ToLowerInvariant());
        if (existingEmail is not null)
            return Result<EmployeeDto>.Failure("An employee with this email already exists.", 409);

        var employee = request.ToEntity();
        await _unitOfWork.Employees.AddAsync(employee);
        await _unitOfWork.SaveChangesAsync();

        // Reload with department
        var created = await _unitOfWork.Employees.GetByIdAsync(employee.Id);
        return Result<EmployeeDto>.Success(EmployeeDto.FromEntity(created!), 201);
    }

    public async Task<Result<EmployeeDto>> UpdateAsync(int id, UpdateEmployeeRequest request)
    {
        var validation = await _updateValidator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            var errors = validation.Errors.Select(e => e.ErrorMessage);
            return Result<EmployeeDto>.Failure(errors, 400);
        }

        var employee = await _unitOfWork.Employees.GetByIdAsync(id);
        if (employee is null)
            return Result<EmployeeDto>.Failure($"Employee with ID {id} was not found.", 404);

        var department = await _unitOfWork.Departments.GetByIdAsync(request.DepartmentId);
        if (department is null)
            return Result<EmployeeDto>.Failure("Selected department does not exist.", 400);

        var emailNormalized = request.Email.Trim().ToLowerInvariant();
        var existingEmail = await _unitOfWork.Employees.GetByEmailAsync(emailNormalized);
        if (existingEmail is not null && existingEmail.Id != id)
            return Result<EmployeeDto>.Failure("Another employee with this email already exists.", 409);

        request.ApplyTo(employee);
        _unitOfWork.Employees.Update(employee);
        await _unitOfWork.SaveChangesAsync();

        var updated = await _unitOfWork.Employees.GetByIdAsync(id);
        return Result<EmployeeDto>.Success(EmployeeDto.FromEntity(updated!));
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(id);
        if (employee is null)
            return Result.Failure($"Employee with ID {id} was not found.", 404);

        _unitOfWork.Employees.Delete(employee);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success(204);
    }
}
