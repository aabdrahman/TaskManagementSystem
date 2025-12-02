using Shared.ApiResponse;
using Shared.DataTransferObjects.User;

namespace Service.Contract;

public interface IUserService
{
    Task<GenericResponse<UserDto>> CreateAsync(CreateUserDto createUserDto);
    Task<GenericResponse<IEnumerable<UserDto>>> GetByUnitAsync(int unitId, bool trackChanges = false, bool hasQueryFilter = true);
    Task<GenericResponse<UserDto>> GetByIdAsync(int Id, bool trackChanges = false, bool hasQueryFilter = true);
    Task<GenericResponse<UserDto>> GetByUsernameAsync(string Username, bool trackChanges = false, bool hasQueryFilter = true);
    Task<GenericResponse<string>> DeleteAsync(string Username, bool isSoftDelete = true);
    Task<GenericResponse<TokenDto>> ValidateUserAsync(UserToLoginDto userToLogin);
    Task<GenericResponse<TokenDto>> RefreshTokenAsync(TokenDto tokenDto);
    Task<GenericResponse<string>> ChangePasswordAsync(ChangeUserPasswordDto changePasswordDto);
    //Task<GenericResponse<string>> ValidateUser();
}
