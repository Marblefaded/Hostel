using HostelDB.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HostelDB.AlfaPruefungDb
{
    public class HostelDbSecond : DbContext
    {
        public HostelDbSecond() { }
        public HostelDbSecond(DbContextOptions<HostelDbSecond> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // builder.Entity<ClaimType>(entity => { entity.ToTable(name: "ClaimType"); });

            builder.Entity<Claim>(entity =>
            {
                entity.ToTable(name: "Claim");
                entity.HasOne(d => d.ClaimTemplate)
                    .WithMany(p => p.ClaimCollection)
                    .HasForeignKey(d => d.ClaimTemplateId)
                    .OnDelete(DeleteBehavior.NoAction);

            });
            builder.Entity<ClaimTemplate>(entity =>
            {
                entity.ToTable(name: "ClaimTemplate");
            });

            builder.Entity<Duty>(entity => { entity.ToTable(name: "Duty"); });

            builder.Entity<Post>(entity => { entity.ToTable(name: "Post"); });

            //builder.Entity<AmsUser>(entity => { entity.ToTable(name: "Role"); });

            builder.Entity<Room>(entity =>
            {
                entity.ToTable(name: "Room");

                entity.HasOne(d => d.RoomType)
                    .WithMany(p => p.RoomCollection)
                    .HasForeignKey(d => d.RoomTypeId);

            });

            builder.Entity<RoomType>(entity => { entity.ToTable(name: "RoomType"); });

            builder.Entity<TelegramUser>(entity =>
            {
                entity.ToTable(name: "TelegramUser");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TelegramUserCollection)
                    .HasForeignKey(d => d.UserId);
            });
            builder.Entity<DutyOrder>(entity =>
            {
                entity.ToTable(name: "DutyOrderList");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.DutyOrderCollection)
                    .HasForeignKey(d => d.RoomId);
            });
            builder.Entity<UserRoom>(entity =>
            {
                entity.ToTable(name: "UserRoom");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserRoomCollection)
                    .HasForeignKey(d => d.UserId);

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.UserRoomCollection)
                    .HasForeignKey(d => d.RoomId);

            });

            builder.Entity<LogMessageEntry>(entity =>
            {
                entity.ToTable("LogApplicationError");

            });
        }
        const string connectionString = "Server=MARBLEFADE\\SQLDEVELOPER;Database=hostel;User Id=admin;Password=wlad1051;TrustServerCertificate=Yes;MultipleActiveResultSets=true;";
        /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            *//*optionsBuilder.UseSqlServer(connectionString, ServerVersion.AutoDetect(connectionString));*//*
            optionsBuilder.UseSqlServer(connectionString: "Server=MARBLEFADE\\SQLDEVELOPER;Database=hostel;User Id=admin;Password=wlad1051;TrustServerCertificate=Yes;MultipleActiveResultSets=true;");
        }*/

    }
}
