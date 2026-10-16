using RaceDayAP1.Models;
using Microsoft.EntityFrameworkCore ;
namespace RaceDayAP1.Data
{
    public class RaceDayContext : DbContext 
    {
        public RaceDayContext(DbContextOptions<RaceDayContext> options)

        : base(options)

        {

        }

        public DbSet<Participant> Participants => Set<Participant>();

        public DbSet<EventCategory> EventCategories => Set<EventCategory>();

        public DbSet<EventOrganizer> EventOrganizers => Set<EventOrganizer>();

        public DbSet<Venue> Venues => Set<Venue>();

        public DbSet<UserAccount> UserAccounts

    => Set<UserAccount>();

        public DbSet<RaceDayAP1.Models.Results> Results
        => Set<RaceDayAP1.Models.Results>();

        public DbSet<RaceEvent> RaceEvents => Set<RaceEvent>();

        public DbSet<Registration> Registrations => Set<Registration>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)

        {
            modelBuilder.Entity<UserAccount>(entity =>

            {

                entity.ToTable("UserAccount");

               

                entity.HasKey(e => e.UserAccount_ID);

                entity.Property(e => e.UserAccount_ID)

                      .ValueGeneratedOnAdd();

                entity.Property(e => e.Email)

                      .HasMaxLength(100)

                      .IsRequired();

                entity.HasIndex(e => e.Email)

                      .IsUnique();

                entity.Property(e => e.PasswordHash)

                      .HasMaxLength(1000)

                      .IsRequired();

                entity.Property(e => e.UserRole)

                      .HasMaxLength(20)

                      .IsRequired();

                entity.Property(e => e.Participant_ID)

                      .HasMaxLength(13);

                entity.Property(e => e.Event_Organizer_ID)

                      .HasMaxLength(13);

            });

            modelBuilder.Entity<Participant>(entity =>

            {

                entity.ToTable("Participant");

                entity.HasKey(e => e.Participant_ID);

                entity.Property(e => e.Participant_ID).HasMaxLength(13);

                entity.Property(e => e.Participant_Name).HasMaxLength(25);

                entity.Property(e => e.Participant_Surname).HasMaxLength(25);

                entity.Property(e => e.Participant_Email)

                      .HasMaxLength(50);

                entity.HasIndex(e => e.Participant_Email).IsUnique();

            });

            modelBuilder.Entity<EventCategory>(entity =>

            {

                entity.ToTable("Event_Category");

                entity.HasKey(e => e.Category_ID);

                entity.Property(e => e.Category_ID).HasMaxLength(13);

                entity.Property(e => e.Event_Category_Type).HasMaxLength(25);

                entity.Property(e => e.Event_Category_Name).HasMaxLength(25);

            });

            modelBuilder.Entity<EventOrganizer>(entity =>

            {

                entity.ToTable("Event_Organizer");

                entity.HasKey(e => e.Event_Organizer_ID);

                entity.Property(e => e.Event_Organizer_ID).HasMaxLength(13);

                entity.Property(e => e.Event_Organizer_Name).HasMaxLength(25);

                entity.Property(e => e.Event_Organizer_Surname).HasMaxLength(25);

                entity.Property(e => e.Event_Organizers_Email).HasMaxLength(50);

            });

            modelBuilder.Entity<Venue>(entity =>

            {

                entity.ToTable("Venue");

                entity.HasKey(e => e.Venue_ID);

                entity.Property(e => e.Venue_ID).HasMaxLength(13);

                entity.Property(e => e.Venue_Name).HasMaxLength(30);

                entity.Property(e => e.Venue_Location).HasMaxLength(30);

            });

            modelBuilder.Entity<RaceDayAP1.Models.Results>(entity =>

            {

                entity.ToTable("Results");

                entity.HasKey(e => e.Results_ID);

                entity.Property(e => e.Results_ID).HasMaxLength(13);

                entity.Property(e => e.Result_Type).HasMaxLength(25);

            });

            modelBuilder.Entity<RaceEvent>(entity =>

            {

                entity.ToTable("RaceEvent");

                entity.HasKey(e => e.Event_ID);

                entity.Property(e => e.Event_ID).HasMaxLength(13);

                entity.Property(e => e.Event_Location).HasMaxLength(100);

                entity.HasOne<EventCategory>()

                    .WithMany()

                    .HasForeignKey(e => e.CategoryID)

                    .HasPrincipalKey(e => e.Category_ID);

                entity.HasOne<EventOrganizer>()

                    .WithMany()

                    .HasForeignKey(e => e.EventOrganizerID)

                    .HasPrincipalKey(e => e.Event_Organizer_ID);

                entity.HasOne<Venue>()

                    .WithMany()

                    .HasForeignKey(e => e.VenueID)

                    .HasPrincipalKey(e => e.Venue_ID);

            });

            modelBuilder.Entity<Registration>(entity =>

            {

                entity.ToTable("Registration");

                entity.HasKey(e => e.Registration_ID);

                entity.Property(e => e.Registration_ID).HasMaxLength(13);

                entity.Property(e => e.Registration_Status)

                      .HasMaxLength(25)

                      .HasDefaultValue("Pending");

                entity.HasOne<Participant>()

                    .WithMany()

                    .HasForeignKey(e => e.ParticipantID)

                    .HasPrincipalKey(e => e.Participant_ID);

                entity.HasOne<RaceEvent>()

                    .WithMany()

                    .HasForeignKey(e => e.EventID)

                    .HasPrincipalKey(e => e.Event_ID);

            });

        }
    }
}
