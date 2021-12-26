using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LemonSharp.User.API.DataAccess;
    
public class UserIdentityDbContext : IdentityDbContext<IdentityUser>
{
    public UserIdentityDbContext(DbContextOptions<UserIdentityDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<IdentityRole>().ToTable(TableConsts.IdentityRoles);
        builder.Entity<IdentityRoleClaim<string>>().ToTable(TableConsts.IdentityRoleClaims);
        builder.Entity<IdentityUserRole<string>>().ToTable(TableConsts.IdentityUserRoles);

        builder.Entity<IdentityUser>().ToTable(TableConsts.IdentityUsers);
        builder.Entity<IdentityUserLogin<string>>().ToTable(TableConsts.IdentityUserLogins);
        builder.Entity<IdentityUserClaim<string>>().ToTable(TableConsts.IdentityUserClaims);
        builder.Entity<IdentityUserToken<string>>().ToTable(TableConsts.IdentityUserTokens);
    }
}


public static class TableConsts
{
    public const string IdentityRoles = "Roles";
    public const string IdentityRoleClaims = "RoleClaims";
    public const string IdentityUserRoles = "UserRoles";
    public const string IdentityUsers = "Users";
    public const string IdentityUserLogins = "UserLogins";
    public const string IdentityUserClaims = "UserClaims";
    public const string IdentityUserTokens = "UserTokens";
}
