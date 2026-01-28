namespace Shared.RequestParameters;

public record class UsersRequestParameter
{
    public int? UnitId { get; set; } = default(int?);
    public string? Name { get; set; } = "";
    public string? Email { get; set; } = "";
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
