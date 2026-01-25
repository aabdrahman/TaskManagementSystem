using Shared.ApiResponse;
using Shared.DataTransferObjects.AuditTrail;

namespace Service.Contract;

public interface IAuditService
{
    Task<GenericResponse<IEnumerable<AuditTrailDto>>> GetAuditTrailForUser(int UserId);
    Task<GenericResponse<IEnumerable<AuditTrailDto>>> GetAuditForRecord(int entityId, string entityName);
}
