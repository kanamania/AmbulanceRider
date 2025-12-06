using System.Text.Json;
using AmbulanceRider.API.DTOs;
using AmbulanceRider.API.Models;
using AmbulanceRider.API.Repositories;

namespace AmbulanceRider.API.Services;

public class TripTypeService : ITripTypeService
{
    private readonly ITripTypeRepository _tripTypeRepository;
    private readonly ITripTypeAttributeRepository _attributeRepository;
    private readonly IPricingMatrixRepository _pricingMatrixRepository;

    public TripTypeService(
        ITripTypeRepository tripTypeRepository,
        ITripTypeAttributeRepository attributeRepository,
        IPricingMatrixRepository pricingMatrixRepository)
    {
        _tripTypeRepository = tripTypeRepository;
        _attributeRepository = attributeRepository;
        _pricingMatrixRepository = pricingMatrixRepository;
    }

    public async Task<IEnumerable<TripTypeDto>> GetAllTripTypesAsync()
    {
        var tripTypes = await _tripTypeRepository.GetAllAsync();
        var result = new List<TripTypeDto>();

        foreach (var tripType in tripTypes)
        {
            var dto = await MapToDtoWithAttributesAsync(tripType);
            result.Add(dto);
        }

        return result;
    }

    public async Task<IEnumerable<TripTypeDto>> GetActiveTripTypesAsync()
    {
        var tripTypes = await _tripTypeRepository.GetActiveAsync();
        var result = new List<TripTypeDto>();

        foreach (var tripType in tripTypes)
        {
            var dto = await MapToDtoWithAttributesAsync(tripType);
            result.Add(dto);
        }

        return result;
    }

    public async Task<TripTypeDto?> GetTripTypeByIdAsync(int id)
    {
        var tripType = await _tripTypeRepository.GetByIdWithAttributesAsync(id);
        if (tripType == null)
            return null;

        return await MapToDtoWithAttributesFromEntityAsync(tripType);
    }

    public async Task<TripTypeDto> CreateTripTypeAsync(CreateTripTypeDto dto)
    {
        var tripType = new TripType
        {
            Name = dto.Name,
            Description = dto.Description,
            Color = dto.Color,
            Icon = dto.Icon,
            IsActive = dto.IsActive,
            DisplayOrder = dto.DisplayOrder,
            CreatedAt = DateTime.UtcNow
        };

        await _tripTypeRepository.AddAsync(tripType);
        return MapToDto(tripType);
    }

    public async Task<TripTypeDto> UpdateTripTypeAsync(int id, UpdateTripTypeDto dto)
    {
        var tripType = await _tripTypeRepository.GetByIdAsync(id);
        if (tripType == null)
            throw new KeyNotFoundException("Trip type not found");

        if (!string.IsNullOrEmpty(dto.Name))
            tripType.Name = dto.Name;

        if (dto.Description != null)
            tripType.Description = dto.Description;

        if (dto.Color != null)
            tripType.Color = dto.Color;

        if (dto.Icon != null)
            tripType.Icon = dto.Icon;

        if (dto.IsActive.HasValue)
            tripType.IsActive = dto.IsActive.Value;

        if (dto.DisplayOrder.HasValue)
            tripType.DisplayOrder = dto.DisplayOrder.Value;

        tripType.UpdatedAt = DateTime.UtcNow;
        await _tripTypeRepository.UpdateAsync(tripType);

        return await MapToDtoWithAttributesAsync(tripType);
    }

    public async Task DeleteTripTypeAsync(int id)
    {
        await _tripTypeRepository.DeleteAsync(id);
    }

    public async Task<TripTypeAttributeDto> CreateAttributeAsync(CreateTripTypeAttributeDto dto)
    {
        var attribute = new TripTypeAttribute
        {
            TripTypeId = dto.TripTypeId,
            Name = dto.Name,
            Label = dto.Label,
            Description = dto.Description,
            DataType = dto.DataType,
            IsRequired = dto.IsRequired,
            DisplayOrder = dto.DisplayOrder,
            Options = dto.Options,
            DefaultValue = dto.DefaultValue,
            ValidationRules = dto.ValidationRules,
            Placeholder = dto.Placeholder,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        await _attributeRepository.AddAsync(attribute);
        return MapAttributeToDto(attribute);
    }

    public async Task<TripTypeAttributeDto> UpdateAttributeAsync(int id, UpdateTripTypeAttributeDto dto)
    {
        var attribute = await _attributeRepository.GetByIdAsync(id);
        if (attribute == null)
            throw new KeyNotFoundException("Attribute not found");

        if (!string.IsNullOrEmpty(dto.Name))
            attribute.Name = dto.Name;

        if (!string.IsNullOrEmpty(dto.Label))
            attribute.Label = dto.Label;

        if (dto.Description != null)
            attribute.Description = dto.Description;

        if (!string.IsNullOrEmpty(dto.DataType))
            attribute.DataType = dto.DataType;

        if (dto.IsRequired.HasValue)
            attribute.IsRequired = dto.IsRequired.Value;

        if (dto.DisplayOrder.HasValue)
            attribute.DisplayOrder = dto.DisplayOrder.Value;

        if (dto.Options != null)
            attribute.Options = dto.Options;

        if (dto.DefaultValue != null)
            attribute.DefaultValue = dto.DefaultValue;

        if (dto.ValidationRules != null)
            attribute.ValidationRules = dto.ValidationRules;

        if (dto.Placeholder != null)
            attribute.Placeholder = dto.Placeholder;

        if (dto.IsActive.HasValue)
            attribute.IsActive = dto.IsActive.Value;

        attribute.UpdatedAt = DateTime.UtcNow;
        await _attributeRepository.UpdateAsync(attribute);

        return MapAttributeToDto(attribute);
    }

    public async Task DeleteAttributeAsync(int id)
    {
        await _attributeRepository.DeleteAsync(id);
    }

    private static TripTypeDto MapToDto(TripType tripType)
    {
        return new TripTypeDto
        {
            Id = tripType.Id,
            Name = tripType.Name,
            Description = tripType.Description,
            Color = tripType.Color,
            Icon = tripType.Icon,
            IsActive = tripType.IsActive,
            DisplayOrder = tripType.DisplayOrder,
            CreatedAt = tripType.CreatedAt,
            Attributes = new List<TripTypeAttributeDto>()
        };
    }

    private async Task<TripTypeDto> MapToDtoWithAttributesFromEntityAsync(TripType tripType)
    {
        var attributeDtos = new List<TripTypeAttributeDto>();
        var attributes = tripType.Attributes
            .Where(a => a.DeletedAt == null)
            .OrderBy(a => a.DisplayOrder)
            .ToList();
        
        foreach (var attr in attributes)
        {
            var dto = await MapAttributeToDtoAsync(attr);
            attributeDtos.Add(dto);
        }
        
        return new TripTypeDto
        {
            Id = tripType.Id,
            Name = tripType.Name,
            Description = tripType.Description,
            Color = tripType.Color,
            Icon = tripType.Icon,
            IsActive = tripType.IsActive,
            DisplayOrder = tripType.DisplayOrder,
            CreatedAt = tripType.CreatedAt,
            Attributes = attributeDtos
        };
    }

    private async Task<TripTypeDto> MapToDtoWithAttributesAsync(TripType tripType)
    {
        var attributes = await _attributeRepository.GetByTripTypeIdAsync(tripType.Id);
        var attributeDtos = new List<TripTypeAttributeDto>();
        
        foreach (var attr in attributes)
        {
            var dto = await MapAttributeToDtoAsync(attr);
            attributeDtos.Add(dto);
        }
        
        return new TripTypeDto
        {
            Id = tripType.Id,
            Name = tripType.Name,
            Description = tripType.Description,
            Color = tripType.Color,
            Icon = tripType.Icon,
            IsActive = tripType.IsActive,
            DisplayOrder = tripType.DisplayOrder,
            CreatedAt = tripType.CreatedAt,
            Attributes = attributeDtos
        };
    }

    private async Task<TripTypeAttributeDto> MapAttributeToDtoAsync(TripTypeAttribute attribute)
    {
        var options = attribute.Options;
        
        if (string.Equals(attribute.DataType, "PricingMatrix", StringComparison.OrdinalIgnoreCase))
        {
            var pricingMatrices = await _pricingMatrixRepository.GetAllAsync();
            var optionsList = pricingMatrices.Select(p => new { value = p.Id, label = p.Name }).ToList();
            options = JsonSerializer.Serialize(optionsList);
        }
        
        return new TripTypeAttributeDto
        {
            Id = attribute.Id,
            TripTypeId = attribute.TripTypeId,
            Name = attribute.Name,
            Label = attribute.Label,
            Description = attribute.Description,
            DataType = attribute.DataType,
            IsRequired = attribute.IsRequired,
            DisplayOrder = attribute.DisplayOrder,
            Options = options,
            DefaultValue = attribute.DefaultValue,
            ValidationRules = attribute.ValidationRules,
            Placeholder = attribute.Placeholder,
            IsActive = attribute.IsActive,
            CreatedAt = attribute.CreatedAt
        };
    }

    private static TripTypeAttributeDto MapAttributeToDto(TripTypeAttribute attribute)
    {
        return new TripTypeAttributeDto
        {
            Id = attribute.Id,
            TripTypeId = attribute.TripTypeId,
            Name = attribute.Name,
            Label = attribute.Label,
            Description = attribute.Description,
            DataType = attribute.DataType,
            IsRequired = attribute.IsRequired,
            DisplayOrder = attribute.DisplayOrder,
            Options = attribute.Options,
            DefaultValue = attribute.DefaultValue,
            ValidationRules = attribute.ValidationRules,
            Placeholder = attribute.Placeholder,
            IsActive = attribute.IsActive,
            CreatedAt = attribute.CreatedAt
        };
    }
}
