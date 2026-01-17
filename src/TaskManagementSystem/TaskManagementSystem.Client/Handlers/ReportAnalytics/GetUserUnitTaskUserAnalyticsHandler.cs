using Shared.ApiResponse;
using Shared.DataTransferObjects.AnalyticsReporting;
using Shared.RequestParameters.Analytics;
using System.Text.Json;
using TaskManagementSystem.Client.Helper;

namespace TaskManagementSystem.Client.Handlers.ReportAnalytics;

public class GetUserUnitTaskUserAnalyticsHandler(IHttpClientFactory httpClientFactory)
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(ClientHelper.SecureClientKey);

    public async Task<GenericResponse<IEnumerable<UserUnitAssignedUserTaskAnalyticsDto>>> Handle(UserUnitTaskUserAnalyticsRequestParameter parameter)
    {
		try
		{
			var queryString = $"api/AnalyticsReporting/getUserUnitAnalytics?StartDate={parameter.StartDate}&EndDate={parameter.EndDate}&UserId={parameter.UserId ?? null}&UnitId={parameter.UnitId ?? null}";

            HttpResponseMessage httpResponse = await _httpClient.GetAsync(queryString);

			string httpResponseContent = await httpResponse.Content.ReadAsStringAsync();

            GenericResponse<IEnumerable<UserUnitAssignedUserTaskAnalyticsDto>>? responseBody = JsonSerializer.Deserialize<GenericResponse<IEnumerable<UserUnitAssignedUserTaskAnalyticsDto>>>(httpResponseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

			return responseBody;
		}
		catch (Exception ex)
		{
			return GenericResponse<IEnumerable<UserUnitAssignedUserTaskAnalyticsDto>>.Failure(null, System.Net.HttpStatusCode.InternalServerError, ex.Message);
		}
    }
}
