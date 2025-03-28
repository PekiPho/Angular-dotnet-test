﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebTemplate.Models;

#nullable disable

namespace WebTemplate.Migrations
{
    [DbContext(typeof(IspitContext))]
    [Migration("20250327131403_asldasjkl")]
    partial class asldasjkl
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("UserModerating", b =>
                {
                    b.Property<int>("CommunityID")
                        .HasColumnType("int");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("CommunityID", "UserID");

                    b.HasIndex("UserID");

                    b.ToTable("UserModerating");
                });

            modelBuilder.Entity("UserSubscribed", b =>
                {
                    b.Property<int>("CommunityID")
                        .HasColumnType("int");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("CommunityID", "UserID");

                    b.HasIndex("UserID");

                    b.ToTable("UserSubscribed");
                });

            modelBuilder.Entity("WebTemplate.Models.Comment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("nvarchar(2000)");

                    b.Property<DateTime>("DateOfComment")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("PostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ReplyToId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("RepostId")
                        .HasColumnType("int");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("Vote")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.HasIndex("ReplyToId");

                    b.HasIndex("RepostId");

                    b.HasIndex("UserId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("WebTemplate.Models.Community", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasMaxLength(20000)
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Communities");
                });

            modelBuilder.Entity("WebTemplate.Models.Media", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("PostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.ToTable("Media");
                });

            modelBuilder.Entity("WebTemplate.Models.Post", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("CommunityId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateOfPost")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasMaxLength(5000)
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("Vote")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CommunityId");

                    b.HasIndex("UserId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("WebTemplate.Models.Repost", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CommunityId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateOfCrossPost")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("PostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Vote")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CommunityId");

                    b.HasIndex("PostId");

                    b.ToTable("Reposts");
                });

            modelBuilder.Entity("WebTemplate.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DateOfAccountCreated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("UserModerating", b =>
                {
                    b.HasOne("WebTemplate.Models.Community", null)
                        .WithMany()
                        .HasForeignKey("CommunityID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebTemplate.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("UserSubscribed", b =>
                {
                    b.HasOne("WebTemplate.Models.Community", null)
                        .WithMany()
                        .HasForeignKey("CommunityID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebTemplate.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WebTemplate.Models.Comment", b =>
                {
                    b.HasOne("WebTemplate.Models.Post", "Post")
                        .WithMany("Comments")
                        .HasForeignKey("PostId");

                    b.HasOne("WebTemplate.Models.Comment", "ReplyTo")
                        .WithMany("Replies")
                        .HasForeignKey("ReplyToId");

                    b.HasOne("WebTemplate.Models.Repost", null)
                        .WithMany("Comments")
                        .HasForeignKey("RepostId");

                    b.HasOne("WebTemplate.Models.User", "User")
                        .WithMany("Comments")
                        .HasForeignKey("UserId");

                    b.Navigation("Post");

                    b.Navigation("ReplyTo");

                    b.Navigation("User");
                });

            modelBuilder.Entity("WebTemplate.Models.Media", b =>
                {
                    b.HasOne("WebTemplate.Models.Post", "Post")
                        .WithMany("Media")
                        .HasForeignKey("PostId");

                    b.Navigation("Post");
                });

            modelBuilder.Entity("WebTemplate.Models.Post", b =>
                {
                    b.HasOne("WebTemplate.Models.Community", "Community")
                        .WithMany("Posts")
                        .HasForeignKey("CommunityId");

                    b.HasOne("WebTemplate.Models.User", "User")
                        .WithMany("Posts")
                        .HasForeignKey("UserId");

                    b.Navigation("Community");

                    b.Navigation("User");
                });

            modelBuilder.Entity("WebTemplate.Models.Repost", b =>
                {
                    b.HasOne("WebTemplate.Models.Community", "Community")
                        .WithMany()
                        .HasForeignKey("CommunityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebTemplate.Models.Post", "Post")
                        .WithMany()
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Community");

                    b.Navigation("Post");
                });

            modelBuilder.Entity("WebTemplate.Models.Comment", b =>
                {
                    b.Navigation("Replies");
                });

            modelBuilder.Entity("WebTemplate.Models.Community", b =>
                {
                    b.Navigation("Posts");
                });

            modelBuilder.Entity("WebTemplate.Models.Post", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Media");
                });

            modelBuilder.Entity("WebTemplate.Models.Repost", b =>
                {
                    b.Navigation("Comments");
                });

            modelBuilder.Entity("WebTemplate.Models.User", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Posts");
                });
#pragma warning restore 612, 618
        }
    }
}
