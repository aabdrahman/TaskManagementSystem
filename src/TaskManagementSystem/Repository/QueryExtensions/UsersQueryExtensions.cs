using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Shared.RequestParameters;

namespace Repository.QueryExtensions;

internal static class UsersQueryExtensions
{
    internal static IQueryable<User> SearchByName(this IQueryable<User> users, UsersRequestParameter usersRequestParameter)
    {
        if(!string.IsNullOrEmpty(usersRequestParameter.Name))
        {
            return users.Where(x => EF.Functions.Like(x.FirstName, usersRequestParameter.Name) || EF.Functions.Like(x.LastName, usersRequestParameter.Name));
        }
        else
        {
            return users;
        }
    }

    internal static IQueryable<User> SearchByUnitId(this IQueryable<User> users, UsersRequestParameter usersRequestParameter)
    {
        if (!usersRequestParameter.UnitId.HasValue)
            return users;

        return users.Where(X => X.UnitId == usersRequestParameter.UnitId);
    }

    internal static IQueryable<User> SearchByEmail(this IQueryable<User> users, UsersRequestParameter userRequestParameter)
    {
        if(string.IsNullOrEmpty(userRequestParameter.Email))
            return users;

        return users.Where(x => x.Email.Contains(userRequestParameter.Email));
    }
}
