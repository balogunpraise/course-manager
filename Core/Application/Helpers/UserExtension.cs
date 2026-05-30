using Core.Domain.Entities.Auth;
using Microsoft.AspNetCore.Identity;

namespace Core.Application.Helpers
{
    public static class UserExtension
    {

        public static async Task<AppUser> GetUserById(this UserManager<AppUser> userManager, Guid userId)
        {
            return await userManager.FindByIdAsync(userId.ToString());
        }

        public static async Task<AppRole> GetRoleById(this RoleManager<AppRole> roleManager, Guid roleId)
        {
            return await roleManager.FindByIdAsync(roleId.ToString());
        }
    }
}
