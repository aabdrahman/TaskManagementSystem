using Shared.ApiResponse;
using Shared.DataTransferObjects.AuditTrail;
using System.Text.Json;
using TaskManagementSystem.Client.Helper;

namespace TaskManagementSystem.Client.Handlers.AuditTrail;

public class GetItemAuditTrailHandler
{
    private HttpClient _HttpClient;

    public GetItemAuditTrailHandler(IHttpClientFactory httpClientFactory)
    {
        _HttpClient = httpClientFactory.CreateClient(ClientHelper.SecureClientKey);
    }

    public async Task<GenericResponse<IEnumerable<AuditTrailDto>>?> Handle(int entityId, string entityName)
    {
        try
        {
            HttpResponseMessage httpResponse = await _HttpClient.GetAsync($"api/AuditTrail?entityName={entityName}&entityId={entityId}");

            string httpResponseContent = await httpResponse.Content.ReadAsStringAsync();

            GenericResponse<IEnumerable<AuditTrailDto>>? responseBody = JsonSerializer.Deserialize<GenericResponse<IEnumerable<AuditTrailDto>>>(httpResponseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            return responseBody;
        }
        catch (Exception ex)
        {
            return GenericResponse<IEnumerable<AuditTrailDto>>.Failure(null, System.Net.HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}
