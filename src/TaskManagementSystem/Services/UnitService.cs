using Contracts;
using Contracts.Infrastructure;
using Service.Contract;
using Shared.DataTransferObjects.Unit;
using Microsoft.EntityFrameworkCore;
using Shared.ApiResponse;
using System.Data.Common;
using Shared.Mapper;
using System.Net;
using System.Text.Json;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Infrastructure.Contracts;

namespace Services;

public sealed class UnitService : IUnitService
{
    private readonly ILoggerManager _loggerManager;
    private readonly IRepositoryManager _repositoryManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IInfrastructureManager _infrastructureManager;
    private readonly IAuditPersistenceService _auditPersistenceService;

    public UnitService(ILoggerManager loggerManager, IRepositoryManager repositoryManager, IHttpContextAccessor httpContextAccessor, IInfrastructureManager infrastructureManager, IAuditPersistenceService auditPersistenceService)
    {
        _loggerManager = loggerManager;
        _repositoryManager = repositoryManager;
        _httpContextAccessor = httpContextAccessor;
        _infrastructureManager = infrastructureManager;
        _auditPersistenceService = auditPersistenceService;
    }

    public async Task<GenericResponse<UnitDto>> CreateAsync(CreateUnitDto newUnitToCreate)
    {
        try
        {
            await _loggerManager.LogInfo($"Creating Unit: {SerializeObjectToString(newUnitToCreate)}");

            bool existingUnitWithName = await _repositoryManager.UnitRepository.GetAllUnits(false, false)
                                                                  .AnyAsync(x => x.NormalizedName == newUnitToCreate.Name.ToUpper());

            if (existingUnitWithName)
            {
                await _loggerManager.LogWarning($"Unit with Name: {newUnitToCreate.Name} already exists.");
                return GenericResponse<UnitDto>.Failure(null, HttpStatusCode.Conflict, $"Unit with Name: {newUnitToCreate.Name} already exists.", null);
            }

            Unit unitToInsert = newUnitToCreate.ToEntity();
            unitToInsert.CreatedBy = $"{_httpContextAccessor.HttpContext.User.FindFirst(x => x.Type.EndsWith("claims/name"))?.Value}-{_httpContextAccessor.HttpContext.User.FindFirst(x => x.Type.EndsWith("claims/serialnumber"))?.Value}";

            await _repositoryManager.UnitRepository.CreateUnit(unitToInsert);
            AuditTrail createAuditTrail = new AuditTrail()
            {
                EntityId = unitToInsert.Id.ToString(),
                EntityName = typeof(Unit).Name,
                PerformedAction = Entities.StaticValues.AuditAction.Created,
                PerformedAt = DateTime.UtcNow.ToLocalTime(),
                ParticipantName = _httpContextAccessor.HttpContext.User.FindFirst(x => x.Type.EndsWith("claims/name"))?.Value ?? "",
                ParticipandIdentification = _httpContextAccessor.HttpContext.User.FindFirst(x => x.Type.EndsWith("claims/serialnumber"))?.Value ?? "0",
                NewValue = SerializeObjectToString(unitToInsert),
                Comment = "Create New Unit"
            };

            //await _repositoryManager.AuditTrailRepository.CreateAuditTrailAsync(createAuditTrail);

            

            try
            {
                await _repositoryManager.SaveChangesAsync();

                bool queueAuditResponse = await _auditPersistenceService.QueueAuditRecord(createAuditTrail);

                await _loggerManager.LogInfo($"Unit Creation Successful - {SerializeObjectToString(unitToInsert)}. Audit Queue status: {queueAuditResponse}");

                await _infrastructureManager.CacheService.AddNewFusionCache<UnitDto>($"Unit-{unitToInsert.Id}", unitToInsert.ToDto());

                await _infrastructureManager.CacheService.RemoveFromCache("Units");

                return GenericResponse<UnitDto>.Success(unitToInsert.ToDto(), HttpStatusCode.Created, "Unit Created Successfully");
            }
            catch (DbUpdateException ex)
            {
                await _loggerManager.LogError(ex, $"A Database Error Occurred Creating Unit: {ex.Message}");
                return GenericResponse<UnitDto>.Failure(null, HttpStatusCode.InternalServerError, "Unit Creation Failed.", new { Message = ex.Message, Description = ex?.InnerException?.Message });
            }

        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, $"An Error Occurred Creating Unit: {ex.Message}");
            return GenericResponse<UnitDto>.Failure(null, HttpStatusCode.InternalServerError, "Unit Creation Failed.", new { Message = ex.Message, Description = ex?.InnerException?.Message });
        }
       
    }

    public async Task<GenericResponse<string>> DeleteAsync(int UnitId, bool isSoftDelete = true)
    {
        try
        {
            await _loggerManager.LogInfo($"Delete Unit: {UnitId}");

            Unit? existingUnit = await _repositoryManager.UnitRepository.GetById(UnitId, true, false)
                                                            .SingleOrDefaultAsync();
            if(existingUnit == null)
            {
                await _loggerManager.LogWarning($"Unit with specified Id: {UnitId} does not exist");
                return GenericResponse<string>.Failure($"No Unit with specified Id exists: {UnitId}", HttpStatusCode.NotFound, "No Unit exists with specified Id", null);
            }

            bool existsUserLinkedToUnit = await _repositoryManager.UserRepository.GetByUnitId(existingUnit.Id, false, true).AnyAsync();

            if(existsUserLinkedToUnit)
            {
                await _loggerManager.LogWarning($"Unit with specified Id: {UnitId} has one or more users still linked to it.");
                return GenericResponse<string>.Failure($"Operation Failed.", HttpStatusCode.Conflict, "Unit has one or more users linked to it.", null);
            }

            Unit auditLogUnit = existingUnit;

            if(isSoftDelete)
            {
                await _loggerManager.LogInfo($"{existingUnit.Name} - marking as inactive");
                _repositoryManager.UnitRepository.UpdateUnit(existingUnit);
            }
            else
            {
                await _loggerManager.LogInfo($"Removing {existingUnit.Name} from database");
                _repositoryManager.UnitRepository.DeleteUnit(existingUnit);
            }

            AuditTrail deleteUnitAuditTrail = new AuditTrail() 
            {
                EntityId = existingUnit.Id.ToString(),
                EntityName = typeof(Unit).Name,
                PerformedAt = DateTime.UtcNow.ToLocalTime(),
                PerformedAction = isSoftDelete ? Entities.StaticValues.AuditAction.SoftDeleted : Entities.StaticValues.AuditAction.HardDeleted,
                ParticipantName = _httpContextAccessor.HttpContext.User.FindFirst(x => x.Type.EndsWith("claims/name"))?.Value ?? "",
                ParticipandIdentification = _httpContextAccessor.HttpContext.User.FindFirst(x => x.Type.EndsWith("claims/serialnumber"))?.Value ?? "0",
                OldValue = SerializeObjectToString(auditLogUnit),
                NewValue = SerializeObjectToString(existingUnit),
                Comment = isSoftDelete ? "Soft Delete Unit" : "Hard Delete Unit"

            };

            //await _repositoryManager.AuditTrailRepository.CreateAuditTrailAsync(deleteUnitAuditTrail);


            await _repositoryManager.SaveChangesAsync();

            bool queueAuditResponse = await _auditPersistenceService.QueueAuditRecord(deleteUnitAuditTrail);


            await _loggerManager.LogInfo(isSoftDelete ? $"Unit with Id: {UnitId} marked as inactive successfully" : $"Unit wit Id: {UnitId} deleted successfully. Queue Audit Response: {queueAuditResponse}");

            await _infrastructureManager.CacheService.RemoveMulipeFromFusionCache($"Unit-{UnitId}", "Units");

            return GenericResponse<string>.Success($"Operation Successful", HttpStatusCode.OK, $"{(isSoftDelete ? "User marked as inactive successfully." : "User Deletion Successful")}");

        }
        catch(DbUpdateException ex)
        {
            await _loggerManager.LogError(ex, $"An Error Occurred Deleting Unit - Id: {UnitId}");
            return GenericResponse<string>.Failure("Error Occurred Deleting Unit.", HttpStatusCode.InternalServerError, ex.Message, null);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, $"An Error Occurred Deleting Unit. Id: {UnitId}");
            return GenericResponse<string>.Failure("Internal Server Error Occurred", HttpStatusCode.InternalServerError, ex.Message, null);
        }
    }

    public async Task<GenericResponse<IEnumerable<UnitDto>>> GetAllUnitsAsync(bool trackChanges, bool hasQueryFilter)
    {
        try
        {
            await _loggerManager.LogInfo($"Fetching All Units....");

            //string cachedDetails = await _infrastructureManager.CacheService.GetFromCache("Units");

            IEnumerable<UnitDto>? cachedRecord = await _infrastructureManager.CacheService.GetFromFusionCache<IEnumerable<UnitDto>>("Units");

            if (cachedRecord is not null)
            {
                await _loggerManager.LogInfo($"Units Fetched successfully from cache: {SerializeObjectToString(cachedRecord)}");

                return GenericResponse<IEnumerable<UnitDto>>.Success(cachedRecord, HttpStatusCode.OK, "Units Fetched Successfully.");
            }


            List<UnitDto> allExistingUnits = await _repositoryManager.UnitRepository.GetAllUnits(trackChanges, hasQueryFilter)
                                               //.Select(x => x.ToDto())
                                               .Select(UnitMapper.ToDtoExpression())
                                               .ToListAsync();
            string serializedUnits = SerializeObjectToString(allExistingUnits);

            //bool updateCacheStaus = await _infrastructureManager.CacheService.CreateNewCache("Units", serializedUnits);

            bool updateCacheStaus = await _infrastructureManager.CacheService.AddNewFusionCache<IEnumerable<UnitDto>>("Units", allExistingUnits);

            await _loggerManager.LogInfo($"Units Fetched Successfully: {serializedUnits}. Update Cache Status: {(updateCacheStaus ? "Successful" : "Failed")}");

            return GenericResponse<IEnumerable<UnitDto>>.Success(allExistingUnits, HttpStatusCode.OK, "Units Fetched Successfully.");
        }
        catch(DbException ex)
        {
            await _loggerManager.LogError(ex, $"Database Error Occurred: {ex.Message}");
            return GenericResponse<IEnumerable<UnitDto>>.Failure(null, HttpStatusCode.InternalServerError, ex.Message, new { ErrorMessage = ex.Message, ErrorDescription = ex?.InnerException?.Message });
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, $"Database Error Occurred: {ex.Message}");
            return GenericResponse<IEnumerable<UnitDto>>.Failure(null, HttpStatusCode.InternalServerError, ex.Message, new { ErrorMessage = ex.Message, ErrorDescription = ex?.InnerException?.Message });
        }
    }

    public async Task<GenericResponse<UnitDto>> GetByIdAsuync(int UnitId)
    {
        await _loggerManager.LogInfo($"Getting Unit with Id: {UnitId}");

        try
        {
            UnitDto? unitFromCache = await _infrastructureManager.CacheService.GetFromFusionCache<UnitDto>($"Unit-{UnitId}");

            if(unitFromCache is not null)
            {
                await _loggerManager.LogInfo($"Unit Fetched successfully from cache - {SerializeObjectToString(unitFromCache)}");
                GenericResponse<UnitDto>.Success(unitFromCache, HttpStatusCode.OK, $"Unit Fetched Successfully.");
            }

            UnitDto? existingUnit = await _repositoryManager.UnitRepository.GetById(UnitId, false)
                                                        //.Select(x => x.ToDto())
                                                        .Select(UnitMapper.ToDtoExpression())
                                                        .SingleOrDefaultAsync();

            await _loggerManager.LogInfo(existingUnit is null ? $"Unit fetched successfully for Id: {UnitId} - {SerializeObjectToString(existingUnit)}" : $"No Unit exists for Id: {UnitId}");

            return existingUnit is null ?
                GenericResponse<UnitDto>.Failure(null, HttpStatusCode.NotFound, $"No Record Exisits for Id: {UnitId}", null) :
                GenericResponse<UnitDto>.Success(existingUnit, HttpStatusCode.OK, $"Unit Fetched Successfully.");
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "An Error Occurred Fetching Unit");
            return GenericResponse<UnitDto>.Failure(null, HttpStatusCode.InternalServerError, $"An Error Occurred Fetching Unit Details", new { ex.Message, Description = ex?.InnerException?.Message });
        }
    }

    public async Task<GenericResponse<UnitDto>> UpdateUnitAsync(UpdateUnitDto updatedUnit)
    {
        try
        {
            string loggedInUser = $"{_httpContextAccessor.HttpContext.User.FindFirst(x => x.Type.EndsWith("claims/name"))?.Value}-{_httpContextAccessor.HttpContext.User.FindFirst(x => x.Type.EndsWith("claims/serialnumber"))?.Value}";
            await _loggerManager.LogInfo($"Update Unit - {SerializeObjectToString(updatedUnit)}. From - {loggedInUser}");

            Unit? unitToUpdate = await _repositoryManager.UnitRepository.GetById(updatedUnit.UnitId).SingleOrDefaultAsync();

            if(unitToUpdate is null)
            {
                await _loggerManager.LogWarning($"Unit with Id; {updatedUnit.UnitId} does not exist.");
                return GenericResponse<UnitDto>.Failure(null, HttpStatusCode.NotFound, "Unit does not exist");
            }

            Unit auditLogUnit = unitToUpdate;

            unitToUpdate.UnitHeadName = updatedUnit.UnitHeadName;
            unitToUpdate.Name = updatedUnit.Name;
            //unitToUpdate.NormalizedName = updatedUnit.Name.ToUpper();

            _repositoryManager.UnitRepository.UpdateUnit(unitToUpdate);

            AuditTrail updateAuditTrail = new AuditTrail()
            {
                EntityId = unitToUpdate.Id.ToString(),
                EntityName = typeof(Unit).Name,
                PerformedAction = Entities.StaticValues.AuditAction.Updated,
                PerformedAt = DateTime.UtcNow.ToLocalTime(),
                ParticipantName = _httpContextAccessor.HttpContext.User.FindFirst(x => x.Type.EndsWith("claims/name"))?.Value ?? "",
                ParticipandIdentification = _httpContextAccessor.HttpContext.User.FindFirst(x => x.Type.EndsWith("claims/serialnumber"))?.Value ?? "0",
                OldValue = SerializeObjectToString(auditLogUnit),
                NewValue = SerializeObjectToString(unitToUpdate),
                Comment = "Update Unit Details"
            };

            //await _repositoryManager.AuditTrailRepository.CreateAuditTrailAsync(updateAuditTrail);

            await _repositoryManager.SaveChangesAsync();

            bool queueAuditResponse = await _auditPersistenceService.QueueAuditRecord(updateAuditTrail);

            await _loggerManager.LogInfo($"Update Unit Successful - {SerializeObjectToString(unitToUpdate.ToDto())} by: {loggedInUser}. Queue Audit Response: {queueAuditResponse}");

            await _infrastructureManager.CacheService.AddNewFusionCache<UnitDto>($"Unit-{unitToUpdate.Id}", unitToUpdate.ToDto());
            await _infrastructureManager.CacheService.RemoveFromCache("Units");

            return GenericResponse<UnitDto>.Success(unitToUpdate.ToDto(), HttpStatusCode.OK, "Unit Updated Successfully.");
        }
        catch (DbUpdateException ex)
        {
            await _loggerManager.LogError(ex, $"An Error Occurred Updating Unit - {SerializeObjectToString(updatedUnit)}");
            return GenericResponse<UnitDto>.Failure(null, HttpStatusCode.InternalServerError, ex.Message, null);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, $"An Error Occurred Updating Unit -  {SerializeObjectToString(updatedUnit)}");
            return GenericResponse<UnitDto>.Failure(null, HttpStatusCode.InternalServerError, ex.Message, null);
        }
    }

    private string SerializeObjectToString(object obj)
    {
        return JsonSerializer.Serialize(obj);
    }
}
