using System;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using EFCore.NamingConventions;
using CryptoTrackApp.src.models;

namespace CryptoTrackApp.src.db
{
    public class CryptoTrackAppContext : DbContext
    {
        public DbSet<User> Users {get; set;}
        public DbSet<Subscription> Subscriptions {get; set;}
        private String CONNECTION = Environment.GetEnvironmentVariable("PGSQL_CONNECTION");
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
            .UseNpgsql(CONNECTION)
            .UseSnakeCaseNamingConvention();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
	  modelBuilder.Entity<User>(user => {
	      user.HasKey( u => u.Id);
	      user.HasAlternateKey( u => u.Email );
	      user.Property( u => u.Status ).IsRequired();
	      //user.Property(u => u.Email).HasMaxLength(200).IsRequired();
		  user.Property( u => u.BirthDate).HasColumnType("date").IsRequired();
	      user.Property(u => u.Password).IsRequired();
	      user.HasMany(u => u.Subscriptions)
		.WithOne(s => s.User)
		.HasForeignKey( s => s.UserId )
		.IsRequired();
	      user.Property( u => u.UserName ).IsRequired();
	    });

	  modelBuilder.Entity<Subscription>(sub => {
	      sub.HasKey(s => s.SubscriptionId);
	      sub.Property(s => s.CurrencyId).IsRequired();
	      sub.Property(s => s.NotificationUmbral).IsRequired();
	      sub.Property(s => s.FollowDate).HasColumnType("timestamp").IsRequired();	    
	      sub.Property(s => s.UnfollowDate).HasColumnType("timestamp");
	      });
	}
	
    }
}

