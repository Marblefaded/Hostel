﻿// <auto-generated />
using System;
using HostelDB.AlfaPruefungDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace HostelDB.Migrations
{
    [DbContext(typeof(HostelDbSecond))]
    [Migration("20240612131916_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("HostelDB.Model.Claim", b =>
                {
                    b.Property<int>("ClaimId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ClaimId"));

                    b.Property<string>("ChangeLog")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimJson")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ClaimTemplateId")
                        .HasColumnType("int");

                    b.Property<int>("ClaimTypeId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<byte[]>("DataClaim")
                        .HasColumnType("varbinary(max)");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("ClaimId");

                    b.HasIndex("ClaimTemplateId");

                    b.HasIndex("ClaimTypeId");

                    b.ToTable("Claim", (string)null);
                });

            modelBuilder.Entity("HostelDB.Model.ClaimTemplate", b =>
                {
                    b.Property<int>("ClaimTemplateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ClaimTemplateId"));

                    b.Property<string>("ChangeLog")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimJson")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ClaimTypeId")
                        .HasColumnType("int");

                    b.Property<byte[]>("DataTemplate")
                        .HasColumnType("varbinary(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<string>("TemplateModelJson")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("ClaimTemplateId");

                    b.HasIndex("ClaimTypeId");

                    b.ToTable("ClaimTemplate", (string)null);
                });

            modelBuilder.Entity("HostelDB.Model.ClaimType", b =>
                {
                    b.Property<int>("ClaimTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ClaimTypeId"));

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("ClaimTypeId");

                    b.ToTable("ClaimType");
                });

            modelBuilder.Entity("HostelDB.Model.Duty", b =>
                {
                    b.Property<int>("DutyMonthId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DutyMonthId"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<int>("Floor")
                        .HasColumnType("int");

                    b.Property<string>("NotCompliteDuty")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("RoomNumber")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("Wing")
                        .IsRequired()
                        .HasMaxLength(1)
                        .HasColumnType("nvarchar(1)");

                    b.HasKey("DutyMonthId");

                    b.ToTable("Duty", (string)null);
                });

            modelBuilder.Entity("HostelDB.Model.DutyOrder", b =>
                {
                    b.Property<int>("DutyOrderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DutyOrderId"));

                    b.Property<int>("Order")
                        .HasColumnType("int");

                    b.Property<int>("RoomId")
                        .HasColumnType("int");

                    b.HasKey("DutyOrderId");

                    b.HasIndex("RoomId");

                    b.ToTable("DutyOrderList", (string)null);
                });

            modelBuilder.Entity("HostelDB.Model.LogMessageEntry", b =>
                {
                    b.Property<int>("LogApplicationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("LogApplicationId"));

                    b.Property<string>("AppVersion")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BrowserInfo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ErrorContext")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ErrorLevel")
                        .HasColumnType("int");

                    b.Property<string>("ErrorMsg")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("InsertDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("UserData")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("LogApplicationId");

                    b.ToTable("LogApplicationError", (string)null);
                });

            modelBuilder.Entity("HostelDB.Model.Post", b =>
                {
                    b.Property<int>("PostId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PostId"));

                    b.Property<string>("ChangeLog")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ListImageJson")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("PostId");

                    b.ToTable("Post", (string)null);
                });

            modelBuilder.Entity("HostelDB.Model.Room", b =>
                {
                    b.Property<int>("RoomId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RoomId"));

                    b.Property<int>("Floor")
                        .HasColumnType("int");

                    b.Property<string>("NumberRoom")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("PeopleMax")
                        .HasColumnType("int");

                    b.Property<int>("RoomTypeId")
                        .HasColumnType("int");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<string>("Wing")
                        .IsRequired()
                        .HasMaxLength(1)
                        .HasColumnType("nvarchar(1)");

                    b.HasKey("RoomId");

                    b.HasIndex("RoomTypeId");

                    b.ToTable("Room", (string)null);
                });

            modelBuilder.Entity("HostelDB.Model.RoomType", b =>
                {
                    b.Property<int>("RoomTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RoomTypeId"));

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("RoomTypeId");

                    b.ToTable("RoomType", (string)null);
                });

            modelBuilder.Entity("HostelDB.Model.TelegramUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("TelegramUserFirstName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("TelegramUserId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("TelegramUserLastName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("TelegramUser", (string)null);
                });

            modelBuilder.Entity("HostelDB.Model.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Secondname")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UserCourse")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("UserDeportament")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UserGroup")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("UserId");

                    b.ToTable("User");
                });

            modelBuilder.Entity("HostelDB.Model.UserRoom", b =>
                {
                    b.Property<int>("UserRoomId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserRoomId"));

                    b.Property<int>("RoomId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("UserRoomId");

                    b.HasIndex("RoomId");

                    b.HasIndex("UserId");

                    b.ToTable("UserRoom", (string)null);
                });

            modelBuilder.Entity("HostelDB.Model.Claim", b =>
                {
                    b.HasOne("HostelDB.Model.ClaimTemplate", "ClaimTemplate")
                        .WithMany("ClaimCollection")
                        .HasForeignKey("ClaimTemplateId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("HostelDB.Model.ClaimType", "ClaimType")
                        .WithMany("ClaimCollection")
                        .HasForeignKey("ClaimTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ClaimTemplate");

                    b.Navigation("ClaimType");
                });

            modelBuilder.Entity("HostelDB.Model.ClaimTemplate", b =>
                {
                    b.HasOne("HostelDB.Model.ClaimType", "ClaimType")
                        .WithMany("ClaimTemplateCollection")
                        .HasForeignKey("ClaimTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ClaimType");
                });

            modelBuilder.Entity("HostelDB.Model.DutyOrder", b =>
                {
                    b.HasOne("HostelDB.Model.Room", "Room")
                        .WithMany("DutyOrderCollection")
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Room");
                });

            modelBuilder.Entity("HostelDB.Model.Room", b =>
                {
                    b.HasOne("HostelDB.Model.RoomType", "RoomType")
                        .WithMany("RoomCollection")
                        .HasForeignKey("RoomTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RoomType");
                });

            modelBuilder.Entity("HostelDB.Model.TelegramUser", b =>
                {
                    b.HasOne("HostelDB.Model.User", "User")
                        .WithMany("TelegramUserCollection")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("HostelDB.Model.UserRoom", b =>
                {
                    b.HasOne("HostelDB.Model.Room", "Room")
                        .WithMany("UserRoomCollection")
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HostelDB.Model.User", "User")
                        .WithMany("UserRoomCollection")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Room");

                    b.Navigation("User");
                });

            modelBuilder.Entity("HostelDB.Model.ClaimTemplate", b =>
                {
                    b.Navigation("ClaimCollection");
                });

            modelBuilder.Entity("HostelDB.Model.ClaimType", b =>
                {
                    b.Navigation("ClaimCollection");

                    b.Navigation("ClaimTemplateCollection");
                });

            modelBuilder.Entity("HostelDB.Model.Room", b =>
                {
                    b.Navigation("DutyOrderCollection");

                    b.Navigation("UserRoomCollection");
                });

            modelBuilder.Entity("HostelDB.Model.RoomType", b =>
                {
                    b.Navigation("RoomCollection");
                });

            modelBuilder.Entity("HostelDB.Model.User", b =>
                {
                    b.Navigation("TelegramUserCollection");

                    b.Navigation("UserRoomCollection");
                });
#pragma warning restore 612, 618
        }
    }
}
