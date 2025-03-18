using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Server.Data;
using System.Security.Claims;

namespace Server.Identity
{
    public class CustomClaimsService : UserClaimsPrincipalFactory<ApiUser>
    {
        public CustomClaimsService(UserManager<ApiUser> userManager, IOptions<IdentityOptions> optionsAccessor) : base(userManager, optionsAccessor)
        {
        }
        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApiUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            var isMinimumAge = user.DateOfBirth.AddYears(18) >= DateTime.Now;
            identity.AddClaim(new Claim(UserClaims.isMinimumAge, isMinimumAge.ToString()));
            identity.AddClaim(new Claim(UserClaims.FullName, $"{user.FirstName} {user.LastName}"));

            foreach (var role in await UserManager.GetRolesAsync(user))
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            return identity;
        }
    }
}
