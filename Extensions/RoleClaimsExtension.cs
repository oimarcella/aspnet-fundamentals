using Blog.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Blog.Extensions
{
    public static class RoleClaimsExtension
    {
        public static IEnumerable<Claim> GetClaims(this User user)
        {
            var userClaims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Email)
            };
            userClaims.AddRange(
                user.Roles.Select(role => new Claim(ClaimTypes.Role, role.Slug))
            );
            return userClaims;
        }
    }
}
