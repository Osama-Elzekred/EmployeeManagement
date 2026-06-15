using EmployeeManagement.Application.Common;
using EmployeeManagement.Application.DTOs.Department;
using EmployeeManagement.Application.Interfaces;
using FluentValidation;

namespace EmployeeManagement.Application.Services;

public class DepartmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateDepartmentRequest> _createValidator;
    private readonly IValidator<UpdateDepartmentRequest> _updateValidator;

    public DepartmentService(
        IUnitOfWork unitOfWork,
        IValidator<CreateDepartmentRequest> createValidator,
        IValidator<UpdateDepartmentRequest> updateValidator)
    {
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<Result<IEnumerable<DepartmentDto>>> GetAllAsync()
    {
        var departments = await _unitOfWork.Departments.GetAllAsync();
        var dtos = departments.Select(DepartmentDto.FromEntity);
        return Result<IEnumerable<DepartmentDto>>.Success(dtos);
    }

    public async Task<Result<DepartmentDto>> GetByIdAsync(int id)
    {
        var department = await _unitOfWork.Departments.GetByIdAsync(id);
        if (department is null)
            return Result<DepartmentDto>.Failure($"Department with ID {id} was not found.", 404);

        return Result<DepartmentDto>.Success(DepartmentDto.FromEntity(department));
    }

    public async Task<Result<DepartmentDto>> CreateAsync(CreateDepartmentRequest request)
    {
        var validation = await _createValidator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            var errors = validation.Errors.Select(e => e.ErrorMessage);
            return Result<DepartmentDto>.Failure(errors, 400);
        }

        var existing = await _unitOfWork.Departments.GetByNameAsync(request.Name.Trim());
        if (existing is not null)
            return Result<DepartmentDto>.Failure("A department with this name already exists.", 409);

        var department = request.ToEntity();
        await _unitOfWork.Departments.AddAsync(department);
        await _unitOfWork.SaveChangesAsync();

        return Result<DepartmentDto>.Success(DepartmentDto.FromEntity(department), 201);
    }

    public async Task<Result<DepartmentDto>> UpdateAsync(int id, UpdateDepartmentRequest request)
    {
        var validation = await _updateValidator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            var errors = validation.Errors.Select(e => e.ErrorMessage);
            return Result<DepartmentDto>.Failure(errors, 400);
        }

        var department = await _unitOfWork.Departments.GetByIdAsync(id);
        if (department is null)
            return Result<DepartmentDto>.Failure($"Department with ID {id} was not found.", 404);

        var existing = await _unitOfWork.Departments.GetByNameAsync(request.Name.Trim());
        if (existing is not null && existing.Id != id)
            return Result<DepartmentDto>.Failure("Another department with this name already exists.", 409);

        request.ApplyTo(department);
        _unitOfWork.Departments.Update(department);
        await _unitOfWork.SaveChangesAsync();

        return Result<DepartmentDto>.Success(DepartmentDto.FromEntity(department));
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var department = await _unitOfWork.Departments.GetByIdAsync(id);
        if (department is null)
            return Result.Failure($"Department with ID {id} was not found.", 404);

        var employees = await _unitOfWork.Employees.SearchAsync(null, id);
        if (employees.Any())
            return Result.Failure("Cannot delete a department that has active employees. Reassign or remove them first.", 400);

        _unitOfWork.Departments.Delete(department);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success(204);
    }
}
