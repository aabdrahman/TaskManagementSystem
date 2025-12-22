using Shared.ApiResponse;
using Shared.DataTransferObjects.Unit;
using System.Text.Json;
using TaskManagementSystem.Client.Helper;

namespace TaskManagementSystem.Client.Handlers.Unit;

public class GetUnitsHandler
{

    private HttpClient _httpClient;

    public GetUnitsHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(ClientHelper.SecureClientKey);
    }

    public async Task<IEnumerable<UnitDto>> Handle()
    {
        try
        {
            var httpResponse = await _httpClient.GetAsync("api/Units?hasQueryFilter=true");

            string httpResponseContent = await httpResponse.Content.ReadAsStringAsync();

            GenericResponse<IEnumerable<UnitDto>>? responseBody = JsonSerializer.Deserialize<GenericResponse<IEnumerable<UnitDto>>>(httpResponseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? null;

            if(responseBody is null)
            {
                return [];
            }

            return responseBody.Data ?? [];

        }
        catch (Exception ex)
        {
            return [];
        }
    }
}
