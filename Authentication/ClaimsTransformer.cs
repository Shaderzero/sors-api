using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using sors.Data;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace sors.Authentication
{
    public class ClaimsTransformer : IClaimsTransformation
    {
        private readonly DataContext _context;

        public ClaimsTransformer(DataContext context)
        {
            _context = context;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var identity = principal.Identities.FirstOrDefault(x => x.IsAuthenticated);
            if (identity == null) return principal;

            var user = identity.Name;
            if (user == null) return principal;
            // строка для удаления наименования домена
            user = user.Substring(user.IndexOf(@"\") + 1);

            //Get user with roles from repository.
            var dbUser = await _context.Accounts
                .Include(u => u.AccountRoles).ThenInclude(r => r.Role)
                .Where(u => u.Name == user)
                .FirstOrDefaultAsync();

            if (dbUser == null)
            {
                return principal;
            }

            var claims = new List<Claim>();

            //The claim identity uses a claim with the claim type below to determine the name property.
            claims.Add(new Claim(@"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", user, "Name"));

            //todo: We should probably create a cache for this
            // Get User Roles from database and add to list of claims.
            foreach (var role in dbUser.AccountRoles.Select((r => r.Role)))
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            var newClaimsIdentity = new ClaimsIdentity(claims, "Kerberos", "", "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");

            var newClaimsPrincipal = new ClaimsPrincipal(newClaimsIdentity);

            return new ClaimsPrincipal(newClaimsPrincipal);
        }
    }
}
