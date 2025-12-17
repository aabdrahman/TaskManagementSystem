using Entities.Models;
using Shared.DataTransferObjects.User;

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
            LastLoginDate = user.LastLoginDate ?? DateTime.MinValue
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
}
