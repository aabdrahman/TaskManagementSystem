using Shared.ApiResponse;
using Shared.DataTransferObjects.AuditTrail;
using System.Text.Json;
using TaskManagementSystem.Client.Helper;

namespace TaskManagementSystem.Client.Handlers.AuditTrail;

public class GetUserAuditTrailHandler
{
    private readonly HttpClient _httpClient;

    public GetUserAuditTrailHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(ClientHelper.SecureClientKey);
    }

    public async Task<GenericResponse<IEnumerable<AuditTrailDto>>?> Handle(int UserId)
    {
        try
        {
            HttpResponseMessage httpResponse = await _httpClient.GetAsync($"api/AuditTrail/getparticipantaudit/{UserId}");

            var httpResponseContent = await httpResponse.Content.ReadAsStringAsync();

            GenericResponse<IEnumerable<AuditTrailDto>>? responseBody = JsonSerializer.Deserialize<GenericResponse<IEnumerable<AuditTrailDto>>>(httpResponseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            return responseBody;
        }
        catch (Exception ex)
        {
            return GenericResponse<IEnumerable<AuditTrailDto>>.Failure(null, System.Net.HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}
