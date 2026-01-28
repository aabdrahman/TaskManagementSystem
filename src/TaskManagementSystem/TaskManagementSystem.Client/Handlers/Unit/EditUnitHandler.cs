using Shared.ApiResponse;
using Shared.DataTransferObjects.Unit;
using System.Net.Http.Json;
using System.Text.Json;
using TaskManagementSystem.Client.Helper;

namespace TaskManagementSystem.Client.Handlers.Unit;

public class EditUnitHandler
{
    private HttpClient _httpClient;

    public EditUnitHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(ClientHelper.SecureClientKey);
    }

    public async Task<(bool isSuccessful, string responseMessage)> Handle(UpdateUnitDto updateUnit)
    {
        try
        {
            HttpResponseMessage httpResponse = await _httpClient.PutAsJsonAsync("api/Units", updateUnit);

            string httpResponseContent = await httpResponse.Content.ReadAsStringAsync();

            GenericResponse<UnitDto>? responseBody = JsonSerializer.Deserialize<GenericResponse<UnitDto>>(httpResponseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? null;

            if(responseBody is null)
            {
                return (false, "An Error Occurred");
            }

            return (responseBody.IsSuccessful, responseBody.Message);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }
}
