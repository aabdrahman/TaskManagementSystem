using Entities.Models;
using Shared.DataTransferObjects.User;
using System.Linq.Expressions;

namespace Shared.Mapper;

public static class UserMapper
{
    public static UserDto ToDto(this User user, int maxDaysToPasswordExpire)
    {
        return new UserDto()
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Username = user.Username,
            DaysToPasswordExpiry = (int)(maxDaysToPasswordExpire - (DateTime.Now.Date - user.LastPasswordChangeDate.Date).TotalDays),
            LastLoginDate = user.LastLoginDate ?? DateTime.MinValue,
            AssignedUnit = user?.AssignedUnit?.NormalizedName ?? ""
        };
    }

    public static User ToEntity(this CreateUserDto createUserDto)
    {
        return new User()
        {
            CreatedBy = "",
            FirstName = createUserDto.FirstName.Trim(),
            LastName = createUserDto.LastName.Trim(),
            Email = createUserDto.Email.Trim().ToUpper(),
            PhoneNumber = createUserDto.PhoneNumber.Trim(),
            UnitId = createUserDto.AssignedUnit,
            Username = createUserDto.Username.Trim().ToUpper(),
            RoleLink = new List<UserRole>() { new UserRole() { CreatedBy = "", RoleId = createUserDto.AssignedRole } }
        };
    }

    public static UserSummaryDto ToSummaryDto(this User user)
    {
        return new UserSummaryDto()
        {
            Id = user.Id,
            Email = user.Email.Trim(),
            FullName = string.Concat(user.FirstName, " ", user.LastName)
        };
    }

    public static Expression<Func<User, UserDto>> ToDtoExpression(int maxDaysToPasswordExpire)
    {
        return user => new UserDto()
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Username = user.Username,
            DaysToPasswordExpiry = (int)(maxDaysToPasswordExpire - (DateTime.Now.Date - user.LastPasswordChangeDate.Date).TotalDays),
            LastLoginDate = user.LastLoginDate ?? DateTime.MinValue,
            AssignedUnit = user.AssignedUnit.NormalizedName
        };
    }

    public static Expression<Func<User, UserSummaryDto>> ToSummaryDtoExpression()
    {
        return user => new UserSummaryDto()
        {
            Id = user.Id,
            Email = user.Email.Trim(),
            FullName = string.Concat(user.FirstName, " ", user.LastName)
        };
    }
}
