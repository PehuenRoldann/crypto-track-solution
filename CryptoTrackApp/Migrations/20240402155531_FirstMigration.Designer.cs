﻿// <auto-generated />
using System;
using CryptoTrackApp.src.db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CryptoTrackApp.Migrations
{
    [DbContext(typeof(CryptoTrackAppContext))]
    [Migration("20240402155531_FirstMigration")]
    partial class FirstMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.27")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("CryptoTrackApp.src.models.Subscription", b =>
                {
                    b.Property<Guid>("SubscriptionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("subscription_id");

                    b.Property<string>("CurrencyId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("currency_id");

                    b.Property<DateTime>("FollowDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("follow_date");

                    b.Property<double>("NotificationUmbral")
                        .HasColumnType("double precision")
                        .HasColumnName("notification_umbral");

                    b.Property<DateTime?>("UnfollowDate")
                        .HasColumnType("timestamp with time 	zone")
                        .HasColumnName("unfollow_date");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("SubscriptionId")
                        .HasName("pk_subscriptions");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_subscriptions_user_id");

                    b.ToTable("subscriptions", (string)null);

                    b.HasData(
                        new
                        {
                            SubscriptionId = new Guid("02a521e0-8625-4fe4-8425-51a36e753c6b"),
                            CurrencyId = "bitcoin",
                            FollowDate = new DateTime(2024, 4, 2, 15, 55, 30, 770, DateTimeKind.Utc).AddTicks(1810),
                            NotificationUmbral = 0.5,
                            UserId = new Guid("7a43853d-b414-4432-b00c-5fd18f77abf6")
                        },
                        new
                        {
                            SubscriptionId = new Guid("f8d07281-b9bd-4137-9f42-a6faecbba632"),
                            CurrencyId = "dia",
                            FollowDate = new DateTime(2024, 4, 2, 15, 55, 30, 770, DateTimeKind.Utc).AddTicks(1814),
                            NotificationUmbral = 0.5,
                            UserId = new Guid("7a43853d-b414-4432-b00c-5fd18f77abf6")
                        },
                        new
                        {
                            SubscriptionId = new Guid("7fefd8e7-65c4-4457-9cd1-b58acd83f36b"),
                            CurrencyId = "solana",
                            FollowDate = new DateTime(2024, 4, 2, 15, 55, 30, 770, DateTimeKind.Utc).AddTicks(1818),
                            NotificationUmbral = 0.5,
                            UserId = new Guid("7a43853d-b414-4432-b00c-5fd18f77abf6")
                        });
                });

            modelBuilder.Entity("CryptoTrackApp.src.models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("BirthDate")
                        .HasColumnType("date")
                        .HasColumnName("birth_date");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("email");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("password");

                    b.Property<bool>("Status")
                        .HasColumnType("boolean")
                        .HasColumnName("status");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("user_name");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.HasAlternateKey("Email")
                        .HasName("ak_users_email");

                    b.ToTable("users", (string)null);

                    b.HasData(
                        new
                        {
                            Id = new Guid("7a43853d-b414-4432-b00c-5fd18f77abf6"),
                            BirthDate = new DateTime(2000, 4, 3, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "pehuen@gmail.com",
                            Password = "Password123!",
                            Status = true,
                            UserName = "Pehuén"
                        });
                });

            modelBuilder.Entity("CryptoTrackApp.src.models.Subscription", b =>
                {
                    b.HasOne("CryptoTrackApp.src.models.User", "User")
                        .WithMany("Subscriptions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_subscriptions_users_user_id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("CryptoTrackApp.src.models.User", b =>
                {
                    b.Navigation("Subscriptions");
                });
#pragma warning restore 612, 618
        }
    }
}
