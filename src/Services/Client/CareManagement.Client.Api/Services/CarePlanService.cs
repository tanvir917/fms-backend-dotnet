using AutoMapper;
using Microsoft.EntityFrameworkCore;
using CareManagement.Client.Api.Data;
using CareManagement.Client.Api.DTOs;
using CareManagement.Client.Api.Models;
using CareManagement.Shared.Messaging;
using CareManagement.Shared.Events;

namespace CareManagement.Client.Api.Services;

public class CarePlanService : ICarePlanService
{
    private readonly ClientDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMessageBus _messageBus;
    private readonly ILogger<CarePlanService> _logger;

    public CarePlanService(ClientDbContext context, IMapper mapper, IMessageBus messageBus, ILogger<CarePlanService> logger)
    {
        _context = context;
        _mapper = mapper;
        _messageBus = messageBus;
        _logger = logger;
    }

    public async Task<IEnumerable<CarePlanDto>> GetCarePlansByClientIdAsync(int clientId)
    {
        var carePlans = await _context.CarePlans
            .Include(cp => cp.Client)
            .Where(cp => cp.ClientId == clientId)
            .OrderByDescending(cp => cp.CreatedAt)
            .ToListAsync();

        return _mapper.Map<IEnumerable<CarePlanDto>>(carePlans);
    }

    public async Task<CarePlanDto?> GetCarePlanByIdAsync(int id)
    {
        var carePlan = await _context.CarePlans
            .Include(cp => cp.Client)
            .FirstOrDefaultAsync(cp => cp.Id == id);

        return carePlan == null ? null : _mapper.Map<CarePlanDto>(carePlan);
    }

    public async Task<CarePlanDto> CreateCarePlanAsync(CreateCarePlanDto createDto, string? createdBy = null)
    {
        var carePlan = _mapper.Map<CarePlan>(createDto);
        carePlan.CreatedBy = createdBy;
        carePlan.UpdatedBy = createdBy;

        _context.CarePlans.Add(carePlan);
        await _context.SaveChangesAsync();

        // Reload with client data
        await _context.Entry(carePlan)
            .Reference(cp => cp.Client)
            .LoadAsync();

        // Publish care plan created event
        try
        {
            var carePlanCreatedEvent = new CarePlanCreatedEvent
            {
                CarePlanId = carePlan.Id,
                ClientId = carePlan.ClientId,
                PlanType = carePlan.Title, // Using title as plan type
                StartDate = carePlan.StartDate,
                EndDate = carePlan.EndDate,
                CreatedAt = carePlan.CreatedAt,
                CreatedBy = createdBy
            };

            await _messageBus.PublishAsync("careplan.created", carePlanCreatedEvent);
            _logger.LogInformation("Published CarePlanCreatedEvent for care plan {CarePlanId}", carePlan.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish CarePlanCreatedEvent for care plan {CarePlanId}", carePlan.Id);
            // Don't throw - the care plan was successfully created, messaging is secondary
        }

        return _mapper.Map<CarePlanDto>(carePlan);
    }

    public async Task<CarePlanDto?> UpdateCarePlanAsync(int id, UpdateCarePlanDto updateDto, string? updatedBy = null)
    {
        var carePlan = await _context.CarePlans
            .Include(cp => cp.Client)
            .FirstOrDefaultAsync(cp => cp.Id == id);

        if (carePlan == null)
        {
            return null;
        }

        _mapper.Map(updateDto, carePlan);
        carePlan.UpdatedBy = updatedBy;

        await _context.SaveChangesAsync();

        return _mapper.Map<CarePlanDto>(carePlan);
    }

    public async Task<bool> DeleteCarePlanAsync(int id)
    {
        var carePlan = await _context.CarePlans.FindAsync(id);
        if (carePlan == null)
        {
            return false;
        }

        _context.CarePlans.Remove(carePlan);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<CarePlanDto?> ActivateCarePlanAsync(ActivateCarePlanDto activateDto, string? updatedBy = null)
    {
        var carePlan = await _context.CarePlans
            .Include(cp => cp.Client)
            .FirstOrDefaultAsync(cp => cp.Id == activateDto.CarePlanId);

        if (carePlan == null)
        {
            return null;
        }

        // Deactivate other care plans for the same client
        var otherCarePlans = await _context.CarePlans
            .Where(cp => cp.ClientId == carePlan.ClientId && cp.Id != carePlan.Id && cp.Status == CarePlanStatus.Active)
            .ToListAsync();

        foreach (var otherPlan in otherCarePlans)
        {
            otherPlan.Status = CarePlanStatus.Inactive;
            otherPlan.UpdatedBy = updatedBy;
        }

        // Activate the selected care plan
        carePlan.Status = CarePlanStatus.Active;
        if (activateDto.StartDate.HasValue)
        {
            carePlan.StartDate = activateDto.StartDate.Value;
        }
        if (activateDto.ReviewDate.HasValue)
        {
            carePlan.ReviewDate = activateDto.ReviewDate.Value;
        }
        carePlan.UpdatedBy = updatedBy;

        await _context.SaveChangesAsync();

        return _mapper.Map<CarePlanDto>(carePlan);
    }
}
