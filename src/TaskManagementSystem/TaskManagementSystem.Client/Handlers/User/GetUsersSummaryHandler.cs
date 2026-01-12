using Shared.ApiResponse;
using Shared.DataTransferObjects.User;
using Shared.RequestParameters;
using System.Text.Json;
using TaskManagementSystem.Client.Helper;

namespace TaskManagementSystem.Client.Handlers.User;

public class GetUsersSummaryHandler
{
    private HttpClient _httpClient;

    public GetUsersSummaryHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(ClientHelper.SecureClientKey);
    }

    public async Task<GenericResponse<PagedItemList<UserDto>>>? Handle(UsersRequestParameter usersRequestParameter)
    {
        try
        {
            HttpResponseMessage httpResponse = await _httpClient.GetAsync($"api/Users/getUserSummaryDetails?UnitId={usersRequestParameter.UnitId}&Name={usersRequestParameter.Name}&Email={usersRequestParameter.Email}&PageNumber={usersRequestParameter.PageNumber}&PageSize={usersRequestParameter.PageSize}&hasQueryFilter=true");

            string httpResponseContent = await httpResponse.Content.ReadAsStringAsync();

            GenericResponse<PagedItemList<UserDto>>? responseBody = JsonSerializer.Deserialize<GenericResponse<PagedItemList<UserDto>>>(httpResponseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? null;

            return responseBody;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<(IEnumerable<UserDto> fetchedUsers, MetaData? fetchedUsersMetaData)> Handle2(UsersRequestParameter usersRequestParameter)
    {
        try
        {
            HttpResponseMessage httpResponse = await _httpClient.GetAsync($"api/Users/getUserSummaryDetails?UnitId={usersRequestParameter.UnitId}&Name={usersRequestParameter.Name}&Email={usersRequestParameter.Email}&PageNumber={usersRequestParameter.PageNumber}&PageSize={usersRequestParameter.PageSize}&hasQueryFilter=true");

            string httpResponseContent = await httpResponse.Content.ReadAsStringAsync();
            Console.WriteLine(httpResponseContent);

            GenericResponse<IEnumerable<UserDto>>? fetchedUsersResponse = JsonSerializer.Deserialize<GenericResponse<IEnumerable<UserDto>>>(httpResponseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? null;

            string responseHeaders = "";
            MetaData? metaData = null;
            if (httpResponse.Headers.TryGetValues(ClientHelper.PaginationKey, out var result) || httpResponse.Content.Headers.TryGetValues(ClientHelper.PaginationKey, out result))
            {
                responseHeaders = result.FirstOrDefault();
                metaData = JsonSerializer.Deserialize<MetaData>(responseHeaders, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? null;
            }

            Console.WriteLine(responseHeaders);

            return fetchedUsersResponse is null ? ([], metaData) : (fetchedUsersResponse.Data, metaData);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return ([],  null);
        }
    }
}
