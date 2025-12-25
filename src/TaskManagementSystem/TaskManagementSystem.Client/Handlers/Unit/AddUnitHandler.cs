using Shared.ApiResponse;
using Shared.DataTransferObjects.Unit;
using System.Net.Http.Json;
using System.Text.Json;
using TaskManagementSystem.Client.Helper;

namespace TaskManagementSystem.Client.Handlers.Unit;

public class AddUnitHandler
{
    private HttpClient _httpClient;
    public AddUnitHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(ClientHelper.SecureClientKey);
    }

    public async Task<(bool isSuccessful, string responseMessage)> Handle(CreateUnitDto unitToCreate)
    {
        try
        {
            var httpResponse = await _httpClient.PostAsJsonAsync("api/Units", unitToCreate);

            string httpResponseContent = await httpResponse.Content.ReadAsStringAsync();

            GenericResponse<UnitDto>? responseBody = JsonSerializer.Deserialize<GenericResponse<UnitDto>>(httpResponseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? null;

            if(responseBody is null)
            {
                return (false, "An Error Occurred.");
            }

            return (responseBody.IsSuccessful, responseBody.Message);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }
}
