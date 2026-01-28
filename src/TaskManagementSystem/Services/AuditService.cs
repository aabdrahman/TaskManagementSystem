using Contracts;
using Contracts.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Service.Contract;
using Shared.ApiResponse;
using Shared.DataTransferObjects.AuditTrail;
using System.Data.Common;
using System.Net.Http;
using Shared.Mapper;

namespace Services;

public sealed class AuditService : IAuditService
{
    private readonly ILoggerManager _loggerManager;
    private readonly IRepositoryManager _repositoryManager;

    public AuditService(ILoggerManager loggerManager, IRepositoryManager repositoryManager)
    {
        _loggerManager = loggerManager;
        _repositoryManager = repositoryManager;
    }

    public async Task<GenericResponse<IEnumerable<AuditTrailDto>>> GetAuditForRecord(int entityId, string entityName)
    {
        try
        {
            await _loggerManager.LogInfo($"Fetching Record Audit Trail. Record Id: {entityId}, Entity: {entityName}");

            IEnumerable<AuditTrailDto> auditTrails = await _repositoryManager.AuditTrailRepository.GetAuditTrail(entityId.ToString(), entityName.Replace("Dto", "", StringComparison.CurrentCultureIgnoreCase))
                                                    .Select(Shared.Mapper.AuditTrailMapper.ToDtoExpression())
                                                    .ToListAsync();

            return GenericResponse<IEnumerable<AuditTrailDto>>.Success(auditTrails, System.Net.HttpStatusCode.OK, "Audit Trail Fetched Successfully.");
        }
        catch(DbException ex)
        {
            await _loggerManager.LogError(ex, "Dataase Error occurred when fetching Audit Trail.");
            return GenericResponse<IEnumerable<AuditTrailDto>>.Failure(null, System.Net.HttpStatusCode.InternalServerError, ex.Message);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error occurred when fetching Audit Trail.");
            return GenericResponse<IEnumerable<AuditTrailDto>>.Failure(null, System.Net.HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<GenericResponse<IEnumerable<AuditTrailDto>>> GetAuditTrailForUser(int UserId)
    {
        try
        {
            await _loggerManager.LogInfo($"Fetching Audit Trail for user: {UserId}");

            var auditTrails = await _repositoryManager.AuditTrailRepository.GetParticipantAudit(UserId.ToString())
                                                .Select(AuditTrailMapper.ToDtoExpression())
                                                .ToListAsync();

            return GenericResponse<IEnumerable<AuditTrailDto>>.Success(auditTrails, System.Net.HttpStatusCode.OK, "Audit Trail Fetched successfully.");
        }
        catch (DbException ex)
        {
            await _loggerManager.LogError(ex, "Dataase Error occurred when fetching Audit Trail.");
            return GenericResponse<IEnumerable<AuditTrailDto>>.Failure(null, System.Net.HttpStatusCode.InternalServerError, ex.Message);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error occurred when fetching Audit Trail.");
            return GenericResponse<IEnumerable<AuditTrailDto>>.Failure(null, System.Net.HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}
