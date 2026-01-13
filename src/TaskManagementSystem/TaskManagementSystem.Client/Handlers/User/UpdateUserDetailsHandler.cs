using Shared.ApiResponse;
using Shared.DataTransferObjects.User;
using System.Net.Http.Json;
using System.Text.Json;
using TaskManagementSystem.Client.Helper;

namespace TaskManagementSystem.Client.Handlers.User;

public class UpdateUserDetailsHandler
{
    private HttpClient _httpClient;
    public UpdateUserDetailsHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(ClientHelper.SecureClientKey);
    }

    public async Task<(bool isSuccessful, string responseMessage)> Handle(UpdateUserDto updateUserDto)
    {
        try
        {
            HttpResponseMessage httpResponse = await _httpClient.PutAsJsonAsync("api/Users", updateUserDto);

            string httpResponseContent = await httpResponse.Content.ReadAsStringAsync();

            GenericResponse<string>? responseBody = JsonSerializer.Deserialize<GenericResponse<string>>(httpResponseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? null;

            if(responseBody is null)
            {
                return (httpResponse.IsSuccessStatusCode, httpResponse.IsSuccessStatusCode ? "Request Successful." : "Request Not Successful.");
            }

            return (responseBody.IsSuccessful, responseBody.Message);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }
}
