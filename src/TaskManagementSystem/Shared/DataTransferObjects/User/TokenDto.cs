namespace Shared.DataTransferObjects.User;

public record class TokenDto
{
    public string RefreshToken { get; set; }
    public string Token { get; set; }
}
