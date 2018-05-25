﻿// <auto-generated />
using Domain.Dictionary;
using Domain.Enums;
using Domain.General;
using Domain.Neural;
using Domain.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Repository;
using System;

namespace Api.Migrations
{
    [DbContext(typeof(ApiDbContext))]
    [Migration("20180507184746_07.05")]
    partial class _0705
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .HasAnnotation("ProductVersion", "2.0.2-rtm-10011");

            modelBuilder.Entity("Domain.Collection.TwitterSource", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("AccountCreatedAt");

                    b.Property<string>("CollectionId");

                    b.Property<DateTime>("CreatedTime");

                    b.Property<string>("Description");

                    b.Property<bool>("GeoEnabled");

                    b.Property<string>("Lang");

                    b.Property<string>("Location");

                    b.Property<string>("ProfileBackgroundImageUrl");

                    b.Property<string>("ProfileImageUrl");

                    b.Property<string>("ProfileLinkColor");

                    b.Property<string>("ProfileSidebarBorderColor");

                    b.Property<string>("ProfileSidebarFillColor");

                    b.Property<string>("ProfileTextColor");

                    b.Property<string>("ScreenName");

                    b.Property<int>("State");

                    b.Property<int>("StatusesCount");

                    b.Property<string>("TimeZone");

                    b.Property<long>("TwitterId");

                    b.Property<DateTime>("UpdatedTime");

                    b.HasKey("Id");

                    b.HasIndex("CollectionId");

                    b.ToTable("TwitterSources");
                });

            modelBuilder.Entity("Domain.Collection.TwitterSourceCollection", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Comments");

                    b.Property<DateTime>("CreatedTime");

                    b.Property<int>("State");

                    b.Property<string>("Title");

                    b.Property<DateTime>("UpdatedTime");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("TwitterSouceCollections");
                });

            modelBuilder.Entity("Domain.Dictionary.DictionaryLanguage", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedTime");

                    b.Property<string>("DictionaryId");

                    b.Property<bool>("IsSelected");

                    b.Property<string>("Language");

                    b.Property<int>("State");

                    b.Property<DateTime>("UpdatedTime");

                    b.HasKey("Id");

                    b.HasIndex("DictionaryId");

                    b.ToTable("DictionaryLanguages");
                });

            modelBuilder.Entity("Domain.Dictionary.TwitterDictionary", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CollectionId");

                    b.Property<DateTime>("CreatedTime");

                    b.Property<int>("MaxDepth");

                    b.Property<int>("MaxStatuses");

                    b.Property<int>("State");

                    b.Property<int>("Status");

                    b.Property<string>("Title");

                    b.Property<DateTime>("UpdatedTime");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("CollectionId");

                    b.HasIndex("UserId");

                    b.ToTable("TwitterDictionaries");
                });

            modelBuilder.Entity("Domain.Neural.NeuralNet", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<byte[]>("Body");

                    b.Property<DateTime>("CreatedTime");

                    b.Property<int>("HiddenCount");

                    b.Property<int>("HiddenLayersCount");

                    b.Property<int>("InputCount");

                    b.Property<bool>("IsTrained");

                    b.Property<string>("Name");

                    b.Property<int>("NetType");

                    b.Property<int>("OutputCount");

                    b.Property<int?>("Seed");

                    b.Property<float>("Skrew");

                    b.Property<int>("State");

                    b.Property<string>("TrainSetId");

                    b.Property<DateTime>("UpdatedTime");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("TrainSetId");

                    b.HasIndex("UserId");

                    b.ToTable("NeuralNets");
                });

            modelBuilder.Entity("Domain.Neural.TrainSet", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedTime");

                    b.Property<byte[]>("Data");

                    b.Property<DateTime>("LastCheckTime");

                    b.Property<string>("Name");

                    b.Property<string>("ScheduleMessage");

                    b.Property<int>("ScheduleStatus");

                    b.Property<string>("SourceId");

                    b.Property<int>("State");

                    b.Property<int>("Type");

                    b.Property<DateTime>("UpdatedTime");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("TrainSets");
                });

            modelBuilder.Entity("Domain.User.EntityUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<DateTime>("Birthday");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<DateTime>("CreatedTime");

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<string>("Locale");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("PictureUrl");

                    b.Property<string>("SecurityStamp");

                    b.Property<int>("State");

                    b.Property<string>("Tel");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<DateTime>("UpdatedTime");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Domain.User.EntityUserSocial", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AccountName");

                    b.Property<string>("ConsumerKey");

                    b.Property<string>("ConsumerSecret");

                    b.Property<DateTime>("CreatedTime");

                    b.Property<long>("ExternalId");

                    b.Property<int>("State");

                    b.Property<string>("Token");

                    b.Property<DateTime>("TokenExpires");

                    b.Property<string>("TokenSecret");

                    b.Property<int>("Type");

                    b.Property<DateTime>("UpdatedTime");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserSocials");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("Domain.Collection.TwitterSource", b =>
                {
                    b.HasOne("Domain.Collection.TwitterSourceCollection", "Collection")
                        .WithMany("Sorces")
                        .HasForeignKey("CollectionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Domain.Collection.TwitterSourceCollection", b =>
                {
                    b.HasOne("Domain.User.EntityUser", "User")
                        .WithMany("TwitterCollections")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Domain.Dictionary.DictionaryLanguage", b =>
                {
                    b.HasOne("Domain.Dictionary.TwitterDictionary", "Dictionary")
                        .WithMany("Languages")
                        .HasForeignKey("DictionaryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Domain.Dictionary.TwitterDictionary", b =>
                {
                    b.HasOne("Domain.Collection.TwitterSourceCollection", "Collection")
                        .WithMany()
                        .HasForeignKey("CollectionId");

                    b.HasOne("Domain.User.EntityUser", "User")
                        .WithMany("TwitterDictionaries")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Domain.Neural.NeuralNet", b =>
                {
                    b.HasOne("Domain.Neural.TrainSet", "TrainSet")
                        .WithMany("NeuralNets")
                        .HasForeignKey("TrainSetId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Domain.User.EntityUser", "User")
                        .WithMany("NeuralNets")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Domain.Neural.TrainSet", b =>
                {
                    b.HasOne("Domain.User.EntityUser", "User")
                        .WithMany("TrainSets")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Domain.User.EntityUserSocial", b =>
                {
                    b.HasOne("Domain.User.EntityUser", "User")
                        .WithMany("Socials")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Domain.User.EntityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Domain.User.EntityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Domain.User.EntityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Domain.User.EntityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
