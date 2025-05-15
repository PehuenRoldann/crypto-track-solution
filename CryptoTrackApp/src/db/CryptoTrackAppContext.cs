using System;
using Microsoft.EntityFrameworkCore;
using CryptoTrackApp.src.models;
using CryptoTrackApp.src.utils;



namespace CryptoTrackApp.src.db
{
    public class CryptoTrackAppContext : DbContext
    {
        public DbSet<User> Users {get; set;}
        public DbSet<Subscription> Subscriptions {get; set;}
		private string _connectionStr = "";
		private Logger _logger = new Logger(); 

		public CryptoTrackAppContext()
        {
			IConfigService configService = JsonConfigService.GetInstance();
			_connectionStr = configService.GetString(ConfigurationsKeys.DbConnection)!;
			_logger.Log($"[CONNECT to postgres DB - Connection string: {_connectionStr}]");
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
            .UseNpgsql(_connectionStr)
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
					.HasForeignKey( s => s.UserId)
					.IsRequired();
				user.Property( u => u.UserName ).IsRequired();
			});

			modelBuilder.Entity<Subscription>(sub => {
				sub.HasKey(s => s.SubscriptionId);
				sub.Property(s => s.CurrencyId).IsRequired();
				sub.Property(s => s.NotificationUmbral).IsRequired();
				sub.Property(s => s.FollowDate).HasColumnType("timestamp with time zone").IsRequired();	    
				sub.Property(s => s.UnfollowDate).HasColumnType("timestamp with time 	zone");
			});

			modelBuilder.Entity<User>().HasData(
				new User {
					Id = Guid.Parse("7a43853d-b414-4432-b00c-5fd18f77abf6"),
					Email = "pehuen@gmail.com",
					Password = "Password123!",
					UserName = "Pehuén",
					BirthDate = new DateTime(year: 2000, month: 4, day: 3),
					Status = true
				}
			);
			modelBuilder.Entity<Subscription>().HasData(
				new Subscription {
					SubscriptionId = Guid.Parse("02a521e0-8625-4fe4-8425-51a36e753c6b"),
					CurrencyId = "bitcoin",
					UserId = Guid.Parse("7a43853d-b414-4432-b00c-5fd18f77abf6"),
					FollowDate = DateTime.UtcNow
				},
				new Subscription {
					SubscriptionId = Guid.Parse("f8d07281-b9bd-4137-9f42-a6faecbba632"),
					CurrencyId = "dia",
					UserId = Guid.Parse("7a43853d-b414-4432-b00c-5fd18f77abf6"),
					FollowDate = DateTime.UtcNow
				},
				new Subscription {
					SubscriptionId = Guid.Parse("7fefd8e7-65c4-4457-9cd1-b58acd83f36b"),
					CurrencyId = "solana",
					UserId = Guid.Parse("7a43853d-b414-4432-b00c-5fd18f77abf6"),
					FollowDate = DateTime.UtcNow
				}
			);
		}
	
    }
}

