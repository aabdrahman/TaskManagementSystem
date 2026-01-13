using Shared.ApiResponse;
using Shared.DataTransferObjects.User;
using System.Text.Json;
using TaskManagementSystem.Client.Helper;

namespace TaskManagementSystem.Client.Handlers.User;

public class GetUserEditDetailsHandler
{
    private HttpClient _httpClient;
    public GetUserEditDetailsHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(ClientHelper.SecureClientKey);
    }

    public async Task<(string responseMessage, UpdateUserDto? userDetailsToUpdate)> Handle(int Id)
    {
        try
        {
            HttpResponseMessage httpResponse = await _httpClient.GetAsync($"api/Users/getUserDetailsToUpdate/{Id}");

            string httpResponseContent = await httpResponse.Content.ReadAsStringAsync();

            GenericResponse<UpdateUserDto>? responseBody = JsonSerializer.Deserialize<GenericResponse<UpdateUserDto>>(httpResponseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? null;

            if(responseBody is null)
            {
                return (httpResponse.IsSuccessStatusCode ? "Request Not Successsful." : "Request Not Successful.", null);
            }

            return (responseBody.Message, responseBody.Data);
        }
        catch (Exception ex)
        {
            return (ex.Message, null);
        }
    }
}
