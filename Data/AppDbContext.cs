using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ApiWithRolesFromScratch.Data
{
    public class ApiWithRoleDbContext
        : IdentityDbContext<IdentityUser>
    {
        public ApiWithRoleDbContext(DbContextOptions dbContextOptions)
            : base(dbContextOptions)
        {

        }
    }
}
