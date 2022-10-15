using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Policy;
using System.Text.Encodings.Web;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using MessagePack.Formatters;
using System.ComponentModel;

namespace Messzendzser.Utils
{
    public class IdentityHelper
    {
       /* private MesszendzserIdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<MesszendzserIdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(MesszendzserIdentityUser)}'. " +
                    $"Ensure that '{nameof(MesszendzserIdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }
        public async void Register(UserManager<MesszendzserIdentityUser> userManager, IUserStore<MesszendzserIdentityUser> userStore, string email, string username, string password)
        {
            var user = CreateUser();

            await userStore.SetUserNameAsync(user, email, CancellationToken.None);
            var result = await userManager.CreateAsync(user, password);            
        }

        public async Task<bool> LogIn(string email, string password,bool rememberMe, IUserStore<MesszendzserIdentityUser> userStore, SignInManager<MesszendzserIdentityUser> signInManager)
        {
            var result = await signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: false);
            return result.Succeeded;
        }

        public async void SignOut(SignInManager<MesszendzserIdentityUser> signInManager)
        {
            await signInManager.SignOutAsync();
        }*/
    }
}
