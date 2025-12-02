using AmbulanceRider.API.DTOs;
using AmbulanceRider.API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AmbulanceRider.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CompaniesController : ControllerBase
{
    private readonly ICompanyRepository _companyRepository;

    public CompaniesController(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CompanyDto>>> GetAllCompanies()
    {
        var companies = await _companyRepository.GetAllAsync();
        var companyDtos = companies.Select(company => new CompanyDto
        {
            Id = company.Id,
            Name = company.Name,
            Description = company.Description,
            ContactEmail = company.ContactEmail,
            ContactPhone = company.ContactPhone,
            Address = company.Address
        });

        return Ok(companyDtos);
    }
}
