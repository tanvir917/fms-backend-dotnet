using CareManagement.Client.Api.DTOs;

namespace CareManagement.Client.Api.Services.Interfaces;

public interface ICarePlanService
{
    Task<IEnumerable<CarePlanDto>> GetCarePlansByClientIdAsync(int clientId);
    Task<CarePlanDto?> GetCarePlanByIdAsync(int id);
    Task<CarePlanDto> CreateCarePlanAsync(CreateCarePlanDto createDto, string? createdBy = null);
    Task<CarePlanDto?> UpdateCarePlanAsync(int id, UpdateCarePlanDto updateDto, string? updatedBy = null);
    Task<bool> DeleteCarePlanAsync(int id);
    Task<CarePlanDto?> ActivateCarePlanAsync(ActivateCarePlanDto activateDto, string? updatedBy = null);
}
