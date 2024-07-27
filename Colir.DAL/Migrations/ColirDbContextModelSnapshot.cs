﻿// <auto-generated />
using System;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DAL.Migrations
{
    [DbContext(typeof(ColirDbContext))]
    partial class ColirDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("DAL.Entities.Attachment", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Filename")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<long?>("MessageId")
                        .HasColumnType("bigint");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasMaxLength(1024)
                        .HasColumnType("nvarchar(1024)");

                    b.Property<long>("SizeInBytes")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("MessageId");

                    b.ToTable("Attachments");
                });

            modelBuilder.Entity("DAL.Entities.LastTimeUserReadChat", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<long>("RoomId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("RoomId");

                    b.HasIndex("UserId");

                    b.ToTable("LastTimeUserReadChats");
                });

            modelBuilder.Entity("DAL.Entities.Message", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<long>("AuthorId")
                        .HasColumnType("bigint");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(4096)
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("EditDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("PostDate")
                        .HasColumnType("datetime2");

                    b.Property<long?>("RepliedMessageId")
                        .HasColumnType("bigint");

                    b.Property<long>("RoomId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("RepliedMessageId");

                    b.HasIndex("RoomId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("DAL.Entities.Reaction", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<long>("AuthorId")
                        .HasColumnType("bigint");

                    b.Property<long>("MessageId")
                        .HasColumnType("bigint");

                    b.Property<string>("Symbol")
                        .HasMaxLength(4)
                        .HasColumnType("nvarchar(4)");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("MessageId");

                    b.ToTable("Reactions");
                });

            modelBuilder.Entity("DAL.Entities.Room", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime?>("ExpiryDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Guid")
                        .HasMaxLength(36)
                        .HasColumnType("nvarchar(36)");

                    b.Property<string>("Name")
                        .HasMaxLength(2147483647)
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("OwnerId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("Guid")
                        .IsUnique()
                        .HasFilter("[Guid] IS NOT NULL");

                    b.HasIndex("OwnerId");

                    b.ToTable("Rooms");
                });

            modelBuilder.Entity("DAL.Entities.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<int>("AuthType")
                        .HasColumnType("int");

                    b.Property<string>("GitHubId")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<int>("HexId")
                        .HasColumnType("int");

                    b.Property<long>("UserSettingsId")
                        .HasColumnType("bigint");

                    b.Property<long>("UserStatisticsId")
                        .HasColumnType("bigint");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("HexId")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("DAL.Entities.UserSettings", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<bool>("StatisticsEnabled")
                        .HasColumnType("bit");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("UserSettings");
                });

            modelBuilder.Entity("DAL.Entities.UserStatistics", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<long>("MessagesSent")
                        .HasColumnType("bigint");

                    b.Property<long>("ReactionsSet")
                        .HasColumnType("bigint");

                    b.Property<long>("RoomsCreated")
                        .HasColumnType("bigint");

                    b.Property<long>("RoomsJoined")
                        .HasColumnType("bigint");

                    b.Property<long>("SecondsSpentInVoice")
                        .HasColumnType("bigint");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("UserStatistics");
                });

            modelBuilder.Entity("DAL.Entities.UserToRoom", b =>
                {
                    b.Property<long>("RoomId")
                        .HasColumnType("bigint");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("RoomId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("UserToRoom");
                });

            modelBuilder.Entity("DAL.Entities.Attachment", b =>
                {
                    b.HasOne("DAL.Entities.Message", "Message")
                        .WithMany("Attachments")
                        .HasForeignKey("MessageId");

                    b.Navigation("Message");
                });

            modelBuilder.Entity("DAL.Entities.LastTimeUserReadChat", b =>
                {
                    b.HasOne("DAL.Entities.Room", "Room")
                        .WithMany()
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("DAL.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Room");

                    b.Navigation("User");
                });

            modelBuilder.Entity("DAL.Entities.Message", b =>
                {
                    b.HasOne("DAL.Entities.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("DAL.Entities.Message", "RepliedTo")
                        .WithMany()
                        .HasForeignKey("RepliedMessageId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("DAL.Entities.Room", "Room")
                        .WithMany()
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("RepliedTo");

                    b.Navigation("Room");
                });

            modelBuilder.Entity("DAL.Entities.Reaction", b =>
                {
                    b.HasOne("DAL.Entities.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DAL.Entities.Message", "Message")
                        .WithMany("Reactions")
                        .HasForeignKey("MessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Message");
                });

            modelBuilder.Entity("DAL.Entities.Room", b =>
                {
                    b.HasOne("DAL.Entities.User", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("DAL.Entities.UserSettings", b =>
                {
                    b.HasOne("DAL.Entities.User", "User")
                        .WithOne("UserSettings")
                        .HasForeignKey("DAL.Entities.UserSettings", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("DAL.Entities.UserStatistics", b =>
                {
                    b.HasOne("DAL.Entities.User", "User")
                        .WithOne("UserStatistics")
                        .HasForeignKey("DAL.Entities.UserStatistics", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("DAL.Entities.UserToRoom", b =>
                {
                    b.HasOne("DAL.Entities.Room", "Room")
                        .WithMany()
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("DAL.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Room");

                    b.Navigation("User");
                });

            modelBuilder.Entity("DAL.Entities.Message", b =>
                {
                    b.Navigation("Attachments");

                    b.Navigation("Reactions");
                });

            modelBuilder.Entity("DAL.Entities.User", b =>
                {
                    b.Navigation("UserSettings")
                        .IsRequired();

                    b.Navigation("UserStatistics")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
