using Shared.ApiResponse;
using System.Text.Json;
using TaskManagementSystem.Client.Helper;

namespace TaskManagementSystem.Client.Handlers.User;

public class DeleteUserHandler
{
    private HttpClient _httpClient;
    public DeleteUserHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(ClientHelper.SecureClientKey);
    }

    public async Task<(bool isSuccessful, string responseMessage)> Handle(string userNameToDelete, bool isSoftDelete = true)
    {
        try
        {
            HttpResponseMessage httpResponse = await _httpClient.DeleteAsync($"api/Users/{userNameToDelete}?isSoftDelete={isSoftDelete}");

            string httpResponseContent = await httpResponse.Content.ReadAsStringAsync();

            GenericResponse<string>? responseBody = JsonSerializer.Deserialize<GenericResponse<string>>(httpResponseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? null;

            if(responseBody is null)
            {
                return (httpResponse.IsSuccessStatusCode, httpResponse.IsSuccessStatusCode ? "Request Successful" : "Request Not Successful");
            }

            return (responseBody.IsSuccessful, responseBody.Message);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }
}
