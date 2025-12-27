using Shared.ApiResponse;
using System.Text.Json;
using TaskManagementSystem.Client.Helper;

namespace TaskManagementSystem.Client.Handlers.Unit;

public class DeleteUnitHandler
{
    private HttpClient _HttpClient;

    public DeleteUnitHandler(IHttpClientFactory httpClientFactory)
    {
        _HttpClient = httpClientFactory.CreateClient(ClientHelper.SecureClientKey);
    }

    public async Task<(bool isSuccessful, string responseMessage)> Handle(int Id)
    {
        try
        {
            var httpResponse = await _HttpClient.DeleteAsync($"api/Units/{Id}?isSoftDelete=false");

            string httpResponseContent = await httpResponse.Content.ReadAsStringAsync();

            GenericResponse<string>? responseBody = JsonSerializer.Deserialize<GenericResponse<string>>(httpResponseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? null;

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
