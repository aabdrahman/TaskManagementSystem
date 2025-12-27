using Contracts;
using Contracts.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Service.Contract;
using Shared.ApiResponse;
using Shared.DataTransferObjects.User;
using System.Data.Common;
using System.Text.Json;
using System.Net;
using Entities.Models;
using Shared.Mapper;
using BCrypt.Net;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Entities.ConfigurationModels;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;

namespace Services;

public sealed class UserService : IUserService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly ILoggerManager _loggerManager;
    private readonly IConfiguration _configuration;
    private readonly JwtConfiguration _jwtConfiguration;
    private readonly IHttpContextAccessor _contextAccessor;

    private User? _loginUser;

    public UserService(IRepositoryManager repositoryManager, ILoggerManager loggerManager, IConfiguration configuration, IOptionsMonitor<JwtConfiguration> jwtConfiurationOptionsMonitor, IHttpContextAccessor contextAccessor)
    {
        _repositoryManager = repositoryManager;
        _loggerManager = loggerManager;
        _configuration = configuration;
        _jwtConfiguration = jwtConfiurationOptionsMonitor.CurrentValue;
        _contextAccessor = contextAccessor;
    }
    public async Task<GenericResponse<UserDto>> CreateAsync(CreateUserDto createUserDto)
    {
        try
        {
            await _loggerManager.LogInfo($"Creating User - {SerializeObject(createUserDto)}");
            
            bool userEmailExists = await _repositoryManager.UserRepository.GetByEmail(createUserDto.Email, false, false)
                                            .AnyAsync();
            if (userEmailExists)
            {
                await _loggerManager.LogWarning($"Email already taken: {createUserDto.Email}");
                return GenericResponse<UserDto>.Failure(null, HttpStatusCode.Conflict, $"Email already exists.", null);
            }
            
            bool userNameExists = await _repositoryManager.UserRepository.GetByUserName(createUserDto.Username, false, false)
                                            .AnyAsync();

            if (userNameExists)
            {
                await _loggerManager.LogWarning($"Username already taken: {createUserDto.Username}");
                return GenericResponse<UserDto>.Failure(null, HttpStatusCode.Conflict, $"Username already taken.", null);
            }

            bool isExistRole = await _repositoryManager.RoleRepository.GetAllRoles(false, true)
                                            .AnyAsync(x => x.Id == createUserDto.AssignedRole);

            if (!isExistRole)
            {
                await _loggerManager.LogWarning($"Invalid Role provided for creating user: {createUserDto.AssignedRole}");
                return GenericResponse<UserDto>.Failure(null, HttpStatusCode.BadRequest, $"Role does not exist", null);
            }

            bool isExistUnit = await _repositoryManager.UnitRepository.GetAllUnits(false, true)
                                            .AnyAsync(x => x.Id == createUserDto.AssignedUnit);
            if (!isExistUnit)
            {
                await _loggerManager.LogWarning($"Invalid Unit provided for creating user: {createUserDto.AssignedUnit}");
                return GenericResponse<UserDto>.Failure(null, HttpStatusCode.Conflict, $"Unit does not exist.", null);
            }

            User userToInsert = createUserDto.ToEntity();
            userToInsert.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(createUserDto.Password.Trim());

            await _repositoryManager.UserRepository.CreateUser(userToInsert);

            await _repositoryManager.SaveChangesAsync();

			bool maxDaysToChangeFromConfig = int.TryParse(_configuration["UserManagement:MaxDaysToChangePassword"] ?? "30", out int daysToLastPasswordChangeValue);

			await _loggerManager.LogInfo($"User Creation Successful - {SerializeObject(userToInsert.ToDto(daysToLastPasswordChangeValue))}");

            return GenericResponse<UserDto>.Success(userToInsert.ToDto(daysToLastPasswordChangeValue), HttpStatusCode.Created, $"User creation successful.");

        }
        catch(DbException ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error - Database");
            return GenericResponse<UserDto>.Failure(null, HttpStatusCode.InternalServerError, $"An Error Occurred - Database", new { ex.Message, Description = ex?.InnerException?.Message });
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error");
            return GenericResponse<UserDto>.Failure(null, HttpStatusCode.InternalServerError, $"An Error Occurred.", new { ex.Message, Description = ex?.InnerException?.Message });
        }
    }

    public async Task<GenericResponse<string>> ChangePasswordAsync(ChangeUserPasswordDto changePasswordDto)
    {
        try
        {
            await _loggerManager.LogInfo($"Change Password for - {SerializeObject(changePasswordDto)}");

            User? existingUser = await _repositoryManager.UserRepository.GetByEmail(changePasswordDto.Email, true, false).SingleOrDefaultAsync();

            if(existingUser is null)
            {
                await _loggerManager.LogWarning($"User with email does not exist - {changePasswordDto.Email}");
                return GenericResponse<string>.Failure("Operation Failed.", HttpStatusCode.NotFound, "No User found with email", null);
            }

            bool isValidOldPassword = BCrypt.Net.BCrypt.EnhancedVerify(changePasswordDto.Password, existingUser.Password);

            if(!isValidOldPassword)
            {
                await _loggerManager.LogWarning($"User with email does exist but provided password is invalid - {changePasswordDto.Email}");
                return GenericResponse<string>.Failure("Operation Failed.", HttpStatusCode.BadRequest, "Invalid Password.", null);
            }

            existingUser.LastPasswordChangeDate = DateTime.UtcNow;
            existingUser.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(changePasswordDto.NewPassword);
            existingUser.IsActive = true;

            _repositoryManager.UserRepository.UpdateUser(existingUser);

            await _repositoryManager.SaveChangesAsync();

            await _loggerManager.LogInfo($"User Password Change Operation Successful for user - {changePasswordDto.Email}");

            return GenericResponse<string>.Success("Operation Successful.", HttpStatusCode.OK, "User Password Changed");
        }
        catch (DbException ex)
        {
            await _loggerManager.LogError(ex, $"Internal Server Error - Database");
            return GenericResponse<string>.Failure("Operation Failed", HttpStatusCode.InternalServerError, $"An Error Occurred - Database", new { ex.Message, Description = ex?.InnerException?.Message });
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, $"Internal Server Error");
            return GenericResponse<string>.Failure("Operation Failed", HttpStatusCode.InternalServerError, $"An Error Occurred - Database", new { ex.Message, Description = ex?.InnerException?.Message });
        }
    }

    public async Task<GenericResponse<string>> DeleteAsync(string Username, bool isSoftDelete = true)
    {
        try
        {
            await _loggerManager.LogInfo($"Deleting User with username - {Username}");

            User? existingUser = await _repositoryManager.UserRepository.GetByUserName(Username, true, false)
                                                .SingleOrDefaultAsync();
            if (existingUser is not null)
            {
                await _loggerManager.LogWarning($"User with specified username does not exist - {Username}");
                return GenericResponse<string>.Failure("Operation Failed", HttpStatusCode.NotFound, $"User with provided username does not exist.", null);
            }

            if(isSoftDelete)
            {
                await _loggerManager.LogInfo($"Marking User with Username: {Username} as inactive");
                existingUser.IsActive = false;
                _repositoryManager.UserRepository.UpdateUser(existingUser);
            }
            else
            {
                _repositoryManager.UserRepository.DeleteUser(existingUser);
            }

            await _repositoryManager.SaveChangesAsync();
            await _loggerManager.LogInfo(isSoftDelete ? $"User with username: {Username} soft deletion successful" : $"User Removed Successfully.");

            return GenericResponse<string>.Success("Operation Successful", HttpStatusCode.OK, isSoftDelete ? "User marked as inactive" : "User deleted successfully.");

        }
        catch (DbException ex)
        {
            await _loggerManager.LogError(ex, $"Internal Server Error - Database");
            return GenericResponse<string>.Failure("Operation Failed", HttpStatusCode.InternalServerError, $"An Error Occurred - Database", new { ex.Message, Description = ex?.InnerException?.Message });
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, $"Internal Server Error");
            return GenericResponse<string>.Failure("Operation Failed", HttpStatusCode.InternalServerError, $"An Error Occurred - Database", new { ex.Message, Description = ex?.InnerException?.Message });
        }
    }

    public async Task<GenericResponse<IEnumerable<UserSummaryDto>>> GetUsersWithSameUnit(int userId)
    {
        try
        {
            await _loggerManager.LogInfo($"Getting Users with same unit as - {userId}");

            bool maxDaysToChangeFromConfig = int.TryParse(_configuration["UserManagement:MaxDaysToChangePassword"] ?? "30", out int daysToLastPasswordChangeValue);

            IQueryable<User> result = await _repositoryManager.UserRepository.GetUsersWithSameUnit(userId);

            List<UserSummaryDto> users = await result.Select(UserMapper.ToSummaryDtoExpression()).ToListAsync();

            return GenericResponse<IEnumerable<UserSummaryDto>>.Success(users, HttpStatusCode.OK, "Users Fetched Successfully.");
        }
        catch (DbException ex)
        {
            await _loggerManager.LogError(ex, $"Internal Server Error - Database");
            return GenericResponse<IEnumerable<UserSummaryDto>>.Failure(null, HttpStatusCode.InternalServerError, $"An Error Occurred - Database", new { ex.Message, Description = ex?.InnerException?.Message });
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, $"Internal Server Error");
            return GenericResponse<IEnumerable<UserSummaryDto>>.Failure(null, HttpStatusCode.InternalServerError, $"An Error Occurred - Database", new { ex.Message, Description = ex?.InnerException?.Message });
        }
    }

    public async Task<GenericResponse<UserDto>> GetByIdAsync(int Id, bool trackChanges = false, bool hasQueryFilter = true)
    {
        try
        {
			bool maxDaysToChangeFromConfig = int.TryParse(_configuration["UserManagement:MaxDaysToChangePassword"] ?? "30", out int daysToLastPasswordChangeValue);

			UserDto? existingUser = await _repositoryManager.UserRepository.GetById(Id, trackChanges, hasQueryFilter)
                                                    //.Select(x => x.ToDto(daysToLastPasswordChangeValue))
                                                    .Select(UserMapper.ToDtoExpression(daysToLastPasswordChangeValue))
                                                    .SingleOrDefaultAsync();
            if (existingUser is null)
            {
                await _loggerManager.LogWarning($"User with specified Id does not exist - {Id}");
                return GenericResponse<UserDto>.Failure(null, HttpStatusCode.NotFound, $"User with provided Id does not exist.", null);
            }

            await _loggerManager.LogInfo($"User with username: {Id} fetched successfully - {SerializeObject(existingUser)}");

            return GenericResponse<UserDto>.Success(existingUser, HttpStatusCode.OK, "User Fetched Successfully.");
        }
        catch (DbException ex)
        {
            await _loggerManager.LogError(ex, $"Internal Server Error - Database");
            return GenericResponse<UserDto>.Failure(null, HttpStatusCode.InternalServerError, $"An Error Occurred - Database", new { ex.Message, Description = ex?.InnerException?.Message });
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, $"Internal Server Error");
            return GenericResponse<UserDto>.Failure(null, HttpStatusCode.InternalServerError, $"An Error Occurred - Database", new { ex.Message, Description = ex?.InnerException?.Message });
        }
    }

    public async Task<GenericResponse<IEnumerable<UserDto>>> GetByUnitAsync(int unitId, bool trackChanges = false, bool hasQueryFilter = true)
    {
        try
        {
            await _loggerManager.LogInfo($"Fetcing Users for unit - {unitId}");

			bool maxDaysToChangeFromConfig = int.TryParse(_configuration["UserManagement:MaxDaysToChangePassword"] ?? "30", out int daysToLastPasswordChangeValue);

			IEnumerable<UserDto> allUsers = await _repositoryManager.UserRepository.GetAllUsers(trackChanges, hasQueryFilter)
                                        .Where(x => x.UnitId == unitId)
                                        //.Select(x => x.ToDto(daysToLastPasswordChangeValue))
                                        .Select(UserMapper.ToDtoExpression(daysToLastPasswordChangeValue))
                                        .ToListAsync();

            await _loggerManager.LogInfo($"Users with Id: {unitId} fetched successfully - {SerializeObject(allUsers)}");

            return GenericResponse<IEnumerable<UserDto>>.Success(allUsers, HttpStatusCode.OK, "Users fetched successfully.");

        }
        catch (DbException ex)
        {
            await _loggerManager.LogError(ex, $"Internal Server Error - Database");
            return GenericResponse<IEnumerable<UserDto>>.Failure(null, HttpStatusCode.InternalServerError, $"An Error Occurred - Database", new { ex.Message, Description = ex?.InnerException?.Message });
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, $"Internal Server Error");
            return GenericResponse<IEnumerable<UserDto>>.Failure(null, HttpStatusCode.InternalServerError, $"An Error Occurred - Database", new { ex.Message, Description = ex?.InnerException?.Message });
        }
    }

    public async Task<GenericResponse<UserDto>> GetByUsernameAsync(string Username, bool trackChanges = false, bool hasQueryFilter = true)
    {
        try
        {
            await _loggerManager.LogInfo($"Getting User with Username - {Username}");

			bool maxDaysToChangeFromConfig = int.TryParse(_configuration["UserManagement:MaxDaysToChangePassword"] ?? "30", out int daysToLastPasswordChangeValue);

			UserDto? existingUser = await _repositoryManager.UserRepository.GetByUserName(Username.Trim(), trackChanges, hasQueryFilter)
                                                    //.Select(x => x.ToDto(daysToLastPasswordChangeValue))
                                                    .Select(UserMapper.ToDtoExpression(daysToLastPasswordChangeValue))
                                                    .FirstOrDefaultAsync();
            if (existingUser is null)
            {
                await _loggerManager.LogWarning($"User with specified username does not exist - {Username}");
                return GenericResponse<UserDto>.Failure(null, HttpStatusCode.NotFound, $"User with provided username does not exist.", null);
            }

            await _loggerManager.LogInfo($"User with username: {Username} fetched successfully - {SerializeObject(existingUser)}");

            return GenericResponse<UserDto>.Success(existingUser, HttpStatusCode.OK, "User Fetched Successfully.");
        }
        catch(DbException ex)
        {
            await _loggerManager.LogError(ex, $"Internal Server Error - Database");
            return GenericResponse<UserDto>.Failure(null, HttpStatusCode.InternalServerError, $"An Error Occurred - Database", new { ex.Message, Description = ex?.InnerException?.Message });
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, $"Internal Server Error");
            return GenericResponse<UserDto>.Failure(null, HttpStatusCode.InternalServerError, $"An Error Occurred - Database", new { ex.Message, Description = ex?.InnerException?.Message });
        }
    }

    public async Task<GenericResponse<TokenDto>> ValidateUserAsync(UserToLoginDto userToLogin)
    {
        try
        {
            await _loggerManager.LogInfo($"Login Request - {SerializeObject(userToLogin)}. From - {_contextAccessor.HttpContext.Request.Headers["Origin".ToString()]}");

            User? existingUserToLogin = await _repositoryManager.UserRepository.GetByEmail(userToLogin.Email, true, true)
                                                                .Include(x => x.AssignedUnit)
                                                                .SingleOrDefaultAsync();

            if (existingUserToLogin == null)
            {
                await _loggerManager.LogWarning($"Invalid User Email. User with Email: {userToLogin.Email} does not exist.");
                return GenericResponse<TokenDto>.Failure(null, HttpStatusCode.NotFound, "Invalid Credentials", null);
            }

            bool isValidPassword = BCrypt.Net.BCrypt.EnhancedVerify(userToLogin.Password.Trim(), existingUserToLogin.Password);

            if (!isValidPassword)
            {
                await _loggerManager.LogInfo($"Invalid User Password- - {JsonSerializer.Serialize(userToLogin)}");
                return GenericResponse<TokenDto>.Failure(null, HttpStatusCode.NotFound, "Invalid Credentials", null);
            }

			int daysToLastPasswordChnage = (int)(DateTime.UtcNow - existingUserToLogin.LastPasswordChangeDate).TotalDays;

			bool maxDaysToChangeFromConfig = int.TryParse(_configuration["UserManagement:MaxDaysToChangePassword"] ?? "30", out int daysToLastPasswordChangeValue);


			if (daysToLastPasswordChnage > daysToLastPasswordChangeValue)
			{
				return GenericResponse<TokenDto>.Failure(null, HttpStatusCode.Locked, "Password Change required", null);
			}

			_loginUser = existingUserToLogin;

            DateTime lastLoginDate = DateTime.Now;

            existingUserToLogin.LastLoginDate = lastLoginDate;
            existingUserToLogin.RefreshToken = GenerateRefreshToken();
            existingUserToLogin.TokenExpirationTime = lastLoginDate.AddMinutes(_jwtConfiguration.sessionTimeoutAfterInMinutes);

            await _repositoryManager.SaveChangesAsync();

            var tokenDetails = await GetToken();

            return GenericResponse<TokenDto>.Success(tokenDetails, HttpStatusCode.OK, "User Login Successful.");
        }
        catch (DbException ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error - Database");
            return GenericResponse<TokenDto>.Failure(null, HttpStatusCode.InternalServerError, "Internal Server Error", new { ex.Message, Description = ex?.InnerException?.Message });
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error");
            return GenericResponse<TokenDto>.Failure(null, HttpStatusCode.InternalServerError, "Internal Server Error", new { ex.Message, Description = ex?.InnerException?.Message });
        }
    }

    public async Task<GenericResponse<IEnumerable<UserSummaryDto>>> GetAllUsers(bool hasQueryFilter = true)
    {
        try
        {
            await _loggerManager.LogInfo($"Fetching Users summary record.");

            IEnumerable<UserSummaryDto> users = await _repositoryManager.UserRepository.GetAllUsers(false, hasQueryFilter)
                                                                            .Select(UserMapper.ToSummaryDtoExpression())
                                                                            //.Select(x => x.ToSummaryDto())
                                                                            .ToListAsync();

            await _loggerManager.LogInfo($"Fetched User Summary records - {SerializeObject(users)}");

            return GenericResponse<IEnumerable<UserSummaryDto>>.Success(users, HttpStatusCode.OK, "Users summary details fetched.");
        }
        catch (DbException ex)
        {
            await _loggerManager.LogError(ex, $"Internal Server Error - Database");
            return GenericResponse<IEnumerable<UserSummaryDto>>.Failure(null, HttpStatusCode.InternalServerError, $"An Error Occurred - Database", new { ex.Message, Description = ex?.InnerException?.Message });
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, $"Internal Server Error");
            return GenericResponse<IEnumerable<UserSummaryDto>>.Failure(null, HttpStatusCode.InternalServerError, $"An Error Occurred - Database", new { ex.Message, Description = ex?.InnerException?.Message });
        }
    }

    public async Task<GenericResponse<TokenDto>> RefreshTokenAsync(TokenDto tokenDto)
    {
        try
        {
            await _loggerManager.LogInfo($"Refresh Token For - {SerializeObject(tokenDto)}");

            var tokenPrincipal = GetPrincipalFromExpiredToken(tokenDto.Token);

            if (tokenPrincipal == null)
            {
                await _loggerManager.LogInfo($"Token Principals could not be fetched.");
                return GenericResponse<TokenDto>.Failure(null, HttpStatusCode.BadRequest, "Invalid Credentialis", null);
            }

            string email = tokenPrincipal.FindFirst(x => x.Type.EndsWith("emailaddress"))?.Value;

            if(email == null)
            {
                await _loggerManager.LogInfo($"Token Principal Email is null");
                return GenericResponse<TokenDto>.Failure(null, HttpStatusCode.BadRequest, "Invalid Credentialis", null);
            }

            User? userWithToken = await _repositoryManager.UserRepository.GetByEmail(email)
                                                            .Include(x => x.AssignedUnit)
                                                            .FirstOrDefaultAsync();  
            if(userWithToken == null || !userWithToken.RefreshToken.Equals(tokenDto.RefreshToken, StringComparison.OrdinalIgnoreCase))
            {
                await _loggerManager.LogWarning($"Invalid User Email. User with Refresh Token: {tokenDto.RefreshToken} does not exist or session expired.");
                return GenericResponse<TokenDto>.Failure(null, HttpStatusCode.NotFound, "Invalid Credentials", null);
            }

            userWithToken.RefreshToken = GenerateRefreshToken();

            _repositoryManager.UserRepository.UpdateUser(userWithToken);

            await _repositoryManager.SaveChangesAsync();

            _loginUser = userWithToken;

            var tokenDetails = await GetToken();

            await _loggerManager.LogInfo($"Refresh token Successful - {SerializeObject(tokenDetails)}");

            return GenericResponse<TokenDto>.Success(tokenDetails, HttpStatusCode.OK, "Token Refreshed Successfully.");
        }
        catch (DbException ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error - Database");
            return GenericResponse<TokenDto>.Failure(null, HttpStatusCode.InternalServerError, "Internal Server Error", new { ex.Message, Description = ex?.InnerException?.Message });
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error");
            return GenericResponse<TokenDto>.Failure(null, HttpStatusCode.InternalServerError, "Internal Server Error", new { ex.Message, Description = ex?.InnerException?.Message });
        }
    }

    private string SerializeObject(object obj)
                        => JsonSerializer.Serialize(obj);

    private string GenerateRefreshToken()
    {
        var rndNum = new byte[32];

        using(var rndGenerator = RandomNumberGenerator.Create())
        {
            rndGenerator.GetBytes(rndNum);
        }

        return Convert.ToBase64String(rndNum);
    }

    private JwtSecurityToken GetSecurityToken(List<Claim> claims, SigningCredentials credentials)
    {

        var tokenOptions = new JwtSecurityToken
        (
            issuer: _jwtConfiguration.validIssuer,
            audience: _contextAccessor.HttpContext.Request.Headers["Origin"].ToString() ?? "TaskManagementApi",
            claims: claims,
            signingCredentials: credentials,
            expires: DateTime.UtcNow.AddSeconds(Convert.ToDouble(_jwtConfiguration.expireAfter))
        );

        return tokenOptions;
    }

    private async Task<TokenDto> GetToken()
    {
        var claims = await GetClaims();
        var credentials = GetCredentials();
        var tokenOptions = GetSecurityToken(claims, credentials);

        string? token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

        if (token == null)
            throw new ArgumentNullException("Toekn Genration Failed.");

        return new TokenDto() { RefreshToken = _loginUser?.RefreshToken!, Token = token };
    }

    private async Task<List<Claim>> GetClaims()
    {
        List<Claim> claims = new List<Claim>() 
        {
            new Claim(ClaimTypes.Email, _loginUser.Email),
            new Claim(ClaimTypes.NameIdentifier, _loginUser.Username),
            new Claim(ClaimTypes.SerialNumber, _loginUser.Id.ToString()),
            new Claim("UnitId", _loginUser.UnitId.ToString()),
            new Claim("AssignedUnit", _loginUser.AssignedUnit.NormalizedName),
            new Claim("FirstName", _loginUser.FirstName),
            new Claim("LastName", _loginUser.LastName),
            new Claim(ClaimTypes.Name, $"{_loginUser.FirstName} {_loginUser.LastName}")
        };

        var userRoles = await _repositoryManager.UserRoleRepository.GetByUserId(_loginUser.Id, false, true)
                                                    .Include(x => x.role)
                                                    .ToListAsync();
        foreach (var role in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.role.NormalizedName));
        }
        
        if(_loginUser.AssignedUnit.UnitHeadName.Equals(string.Concat(_loginUser.FirstName, " ", _loginUser.LastName), StringComparison.OrdinalIgnoreCase))
        {
            claims.Add(new Claim("isUnitHead", "true"));
        }

        return claims;

    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false,
            //ValidAudience = _jwtConfiguration.validAudience,
            ValidAudiences = _jwtConfiguration.validAudience,
            ValidIssuer = _jwtConfiguration.validIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Secret-Key"]!))
        };
        SecurityToken securityToken;
        var tokenHandler = new JwtSecurityTokenHandler();

        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

        var jwtSecurityToken = securityToken as JwtSecurityToken;

        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            return null!;
        }

        return principal;
    }

    private SigningCredentials GetCredentials()
    {
        string secretKey = _configuration["Secret-Key"] ?? throw new ArgumentNullException("No Secret Key.");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        return new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
    }


}
