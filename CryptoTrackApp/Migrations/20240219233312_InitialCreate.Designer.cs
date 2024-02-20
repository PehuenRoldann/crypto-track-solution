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
    [Migration("20240219233312_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("CryptoTrackApp.src.models.Subscription", b =>
                {
                    b.Property<Guid>("SubscriptionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("subscription_id");

                    b.Property<Guid>("CurrencyId")
                        .HasColumnType("uuid")
                        .HasColumnName("currency_id");

                    b.Property<DateTime>("FollowDate")
                        .HasColumnType("timestamp")
                        .HasColumnName("follow_date");

                    b.Property<double>("NotificationUmbral")
                        .HasColumnType("double precision")
                        .HasColumnName("notification_umbral");

                    b.Property<DateTime?>("UnfollowDate")
                        .HasColumnType("timestamp")
                        .HasColumnName("unfollow_date");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("SubscriptionId")
                        .HasName("pk_subscriptions");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_subscriptions_user_id");

                    b.ToTable("subscriptions", (string)null);
                });

            modelBuilder.Entity("CryptoTrackApp.src.models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("email");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("password");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.ToTable("users", (string)null);
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
