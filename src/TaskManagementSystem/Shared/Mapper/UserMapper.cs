using Entities.Models;
using Shared.DataTransferObjects.User;

namespace Shared.Mapper;

public static class UserMapper
{
    public static UserDto ToDto(this User user)
    {
        return new UserDto()
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Username = user.Username
        };
    }

    public static User ToEntity(this CreateUserDto createUserDto)
    {
        return new User()
        {
            CreatedBy = "",
            FirstName = createUserDto.FirstName,
            LastName = createUserDto.LastName,
            Email = createUserDto.Email,
            PhoneNumber = createUserDto.PhoneNumber,
            UnitId = createUserDto.AssignedUnit,
            Username = createUserDto.Username,
            RoleLink = new List<UserRole>() { new UserRole() { CreatedBy = "", RoleId = createUserDto.AssignedRole } }
        };
    }
}
