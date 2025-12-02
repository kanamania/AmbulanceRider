using AmbulanceRider.API.Data;
using AmbulanceRider.API.DTOs;
using AmbulanceRider.API.Models;
using AmbulanceRider.API.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AmbulanceRider.API.Services;

public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _companyRepository;
    private readonly ApplicationDbContext _context;

    public CompanyService(ICompanyRepository companyRepository, ApplicationDbContext context)
    {
        _companyRepository = companyRepository;
        _context = context;
    }

    public async Task<IEnumerable<CompanyDto>> GetAllAsync()
    {
        var companies = await _companyRepository.GetAllAsync();
        return companies.Select(MapToDto);
    }

    public async Task<CompanyDto?> GetByIdAsync(int id)
    {
        var company = await _companyRepository.GetByIdAsync(id);
        return company == null ? null : MapToDto(company);
    }

    public async Task<CompanyDto> CreateAsync(CreateCompanyDto dto, Guid createdBy)
    {
        await EnsureUniqueNameAsync(dto.Name);

        var company = new Company
        {
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim(),
            ContactEmail = dto.ContactEmail?.Trim(),
            ContactPhone = dto.ContactPhone?.Trim(),
            Address = dto.Address?.Trim(),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        await _companyRepository.AddAsync(company);
        return MapToDto(company);
    }

    public async Task<CompanyDto> UpdateAsync(int id, UpdateCompanyDto dto, Guid updatedBy)
    {
        var company = await _companyRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Company not found");

        await EnsureUniqueNameAsync(dto.Name, id);

        company.Name = dto.Name.Trim();
        company.Description = dto.Description?.Trim();
        company.ContactEmail = dto.ContactEmail?.Trim();
        company.ContactPhone = dto.ContactPhone?.Trim();
        company.Address = dto.Address?.Trim();
        company.UpdatedAt = DateTime.UtcNow;
        company.UpdatedBy = updatedBy;

        await _context.SaveChangesAsync();
        return MapToDto(company);
    }

    public async Task DeleteAsync(int id, Guid deletedBy)
    {
        var company = await _companyRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Company not found");

        if (company.Users.Any())
        {
            throw new InvalidOperationException("Cannot delete a company with assigned users.");
        }

        company.DeletedAt = DateTime.UtcNow;
        company.DeletedBy = deletedBy;
        await _context.SaveChangesAsync();
    }

    private async Task EnsureUniqueNameAsync(string name, int? excludeId = null)
    {
        var normalized = name.Trim().ToLowerInvariant();
        var query = _context.Companies.AsQueryable();

        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }

        var exists = await query.AnyAsync(c => c.DeletedAt == null && c.Name.ToLower() == normalized);
        if (exists)
        {
            throw new InvalidOperationException("A company with this name already exists.");
        }
    }

    private static CompanyDto MapToDto(Company company)
    {
        return new CompanyDto
        {
            Id = company.Id,
            Name = company.Name,
            Description = company.Description,
            ContactEmail = company.ContactEmail,
            ContactPhone = company.ContactPhone,
            Address = company.Address
        };
    }
}
