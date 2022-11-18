using Messzendzser.Model.DB.Models;
using System.Security.Claims;

namespace Messzendzser.Utils
{
    public static class Extensions
    {
        public static User ToUser(this ClaimsPrincipal principal)
        {
            var userId = Convert.ToInt32(principal.FindFirstValue(ClaimTypes.NameIdentifier));
            var userName = principal.FindFirstValue(ClaimTypes.Name);
            var email = principal.FindFirstValue(ClaimTypes.Email);
            return new User()
            {
                Id = userId,
                Email = email,
                UserName = userName
            };
        }
    }
}
