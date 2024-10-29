using Suo.Autorization.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using Suo.Autorization.SingleService.Core.Auth.Identity;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace Ams.Pm.Wasm.Core.Auth.Identity;

public class ApplicationAuthDbContext : IdentityDbContext<AmsUser, AmsRole, string, IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, AmsRoleClaim, IdentityUserToken<string>>
{
    ApplicationAuthDbContext() { }
    public DbSet<AmsRefreshTokens> RefreshTokens { get; set; }
    public DbSet<LogMessageEntry> LogMessageEntry { get; set; }
    public ApplicationAuthDbContext(DbContextOptions<ApplicationAuthDbContext> options) : base(options)
    {


    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AmsUser>(entity =>
        {
            entity.ToTable(name: "AspNetUsers");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasMany(x => x.Tokens).WithOne(x => x.User).HasForeignKey(x => x.UserId);
        });

        builder.Entity<AmsRole>(entity =>
        {
            entity.ToTable(name: "AspNetRoles");
        });
        builder.Entity<IdentityUserRole<string>>(entity =>
        {
            entity.ToTable("AspNetUserRoles");
        });

        builder.Entity<IdentityUserClaim<string>>(entity =>
        {
            entity.ToTable("AspNetUserClaims");
        });

        builder.Entity<IdentityUserLogin<string>>(entity =>
        {
            entity.ToTable("AspNetUserLogins");
        });

        builder.Entity<AmsRoleClaim>(entity =>
        {
            entity.ToTable(name: "AspNetRoleClaims");

            entity.HasOne(d => d.Role)
                .WithMany(p => p.RoleClaims)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<IdentityUserToken<string>>(entity =>
        {
            entity.ToTable("AspNetUserTokens");
        });

        builder.Entity<AmsRefreshTokens>(entity =>
        {
            entity.ToTable(name: "AspRefreshTokens");

            entity.Property(e => e.RefreshTokenId).ValueGeneratedOnAdd();

            entity.HasOne(x => x.User).WithMany(x => x.Tokens).HasForeignKey(x => x.UserId);
            //entity.HasOne(x => x.tok).WithMany(x => x.Tokens).HasForeignKey(x=> x.UserId);
        });
    }



    /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        optionsBuilder.UseSqlServer(connectionString: "Server=MARBLEFADE\\SQLDEVELOPER;Database=hostel;User Id=admin;Password=wlad1051;TrustServerCertificate=Yes;MultipleActiveResultSets=true;");
    }*/

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    //const string connectionString = "Server=localhost; User ID=hosteluser; Password=test12345; Database=Hostel;";

    //    //optionsBuilder.UseSqlServer(connectionString: "Server=DESKTOP-VJPGPQ5\\SQLEXPRESS;Database=HstelCibGUTI;User Id=admin2;Password=123;MultipleActiveResultSets=True;TrustServerCertificate =Yes");
    //    optionsBuilder.UseSqlServer(connectionString: "Server=92.205.60.239;Port=8506; User ID=hostel; Password=Sx2p1Te6BtACEwQjwE; Database=hostel;");
    //}
}
public static class Helper<T>
{
    public static List<T> ExecuteSQLQuery(DbContext context, string sql, Func<DbDataReader, T> map)
    {
        using (var cmd = context.Database.GetDbConnection().CreateCommand())
        {
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            context.Database.OpenConnection();
            using (var result = cmd.ExecuteReader())
            {
                var List = new List<T>();
                while (result.Read())
                {
                    List.Add(map(result));
                }
                return List;
            }
            context.Database.CloseConnection();
        }
        return null;
    }
}