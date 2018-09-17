using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using EventManager.DataModel.Models.Mapping;
using Repository.Pattern.Ef6;

namespace EventManager.DataModel.Models
{
	public partial class GameManagerContext : DataContext
    {
        static GameManagerContext()
        {
            Database.SetInitializer<GameManagerContext>(null);
        }

        public GameManagerContext()
            : base("Name=GameManagerContext")
        {
        }

		public GameManagerContext(string context)
			: base(context)
		{
			Database.SetInitializer<GameManagerContext>(null);
		}

        public DbSet<AspNetRole> AspNetRoles { get; set; }
        public DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public DbSet<AspNetUser> AspNetUsers { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventCampaign> EventCampaigns { get; set; }
        public DbSet<EventCategory> EventCategories { get; set; }
        public DbSet<Gift> Gifts { get; set; }
		public DbSet<EventRegister> EventRegisters { get; set; }
		public DbSet<UserGiftRedeem> UserGiftRedeems { get; set; }
        public DbSet<sysdiagram> sysdiagrams { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new AspNetRoleMap());
            modelBuilder.Configurations.Add(new AspNetUserClaimMap());
            modelBuilder.Configurations.Add(new AspNetUserLoginMap());
            modelBuilder.Configurations.Add(new AspNetUserMap());
            modelBuilder.Configurations.Add(new CityMap());
            modelBuilder.Configurations.Add(new EventMap());
            modelBuilder.Configurations.Add(new EventCampaignMap());
            modelBuilder.Configurations.Add(new EventCategoryMap());
            modelBuilder.Configurations.Add(new EventRegisterMap());
            modelBuilder.Configurations.Add(new sysdiagramMap());
			modelBuilder.Configurations.Add(new MessageContentMap());
			modelBuilder.Configurations.Add(new MessageContentSentMap());
			modelBuilder.Configurations.Add(new GifMap());
			modelBuilder.Configurations.Add(new UserGiftRedeemMap());
        }
    }
}
