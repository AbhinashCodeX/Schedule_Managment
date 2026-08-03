using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Schedule_Management.Models;

public partial class ScheduleManagementDbContext : DbContext
{
    public ScheduleManagementDbContext()
    {
    }

    public ScheduleManagementDbContext(DbContextOptions<ScheduleManagementDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActivityType> ActivityTypes { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<CoachAvailability> CoachAvailabilities { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<District> Districts { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Database=ScheduleManagementDB;Integrated Security=True;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActivityType>(entity =>
        {
            entity.HasIndex(e => e.ActivityName, "UQ_ActivityTypes_ActivityName").IsUnique();

            entity.Property(e => e.ActivityName).HasMaxLength(100);
            entity.Property(e => e.CreatedOn)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_ActivityTypes_CreatedOn");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_ActivityTypes_IsActive");
            entity.Property(e => e.ModifiedOn).HasPrecision(0);
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasIndex(e => new { e.AvailabilityId, e.BookingStatus }, "IX_Bookings_AvailabilityId");

            entity.HasIndex(e => new { e.UserId, e.BookingStatus }, "IX_Bookings_UserId");

            entity.Property(e => e.BookedOn)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_Bookings_BookedOn");
            entity.Property(e => e.BookingStatus)
                .HasMaxLength(20)
                .HasDefaultValue("Confirmed", "DF_Bookings_BookingStatus");
            entity.Property(e => e.CancellationReason).HasMaxLength(300);
            entity.Property(e => e.CancelledOn).HasPrecision(0);
            entity.Property(e => e.CreatedOn)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_Bookings_CreatedOn");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Bookings_IsActive");
            entity.Property(e => e.ModifiedOn).HasPrecision(0);

            entity.HasOne(d => d.Availability).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.AvailabilityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bookings_CoachAvailabilities");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bookings_Users");
        });

        modelBuilder.Entity<CoachAvailability>(entity =>
        {
            entity.HasKey(e => e.AvailabilityId);

            entity.HasIndex(e => new { e.ActivityTypeId, e.CoachId, e.AvailableDate, e.IsBooked, e.IsActive }, "IX_CoachAvailabilities_Search");

            entity.HasIndex(e => new { e.CoachId, e.ActivityTypeId, e.AvailableDate, e.StartTime, e.EndTime }, "UQ_CoachAvailabilities_Schedule").IsUnique();

            entity.Property(e => e.CreatedOn)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_CoachAvailabilities_CreatedOn");
            entity.Property(e => e.EndTime).HasPrecision(0);
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_CoachAvailabilities_IsActive");
            entity.Property(e => e.ModifiedOn).HasPrecision(0);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.StartTime).HasPrecision(0);

            entity.HasOne(d => d.ActivityType).WithMany(p => p.CoachAvailabilities)
                .HasForeignKey(d => d.ActivityTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CoachAvailabilities_ActivityTypes");

            entity.HasOne(d => d.Coach).WithMany(p => p.CoachAvailabilities)
                .HasForeignKey(d => d.CoachId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CoachAvailabilities_Users");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasIndex(e => e.Iso2, "UQ_Countries_ISO2").IsUnique();

            entity.Property(e => e.CountryName).HasMaxLength(150);
            entity.Property(e => e.CreatedOn)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_Countries_CreatedOn");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Countries_IsActive");
            entity.Property(e => e.Iso2)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ISO2");
            entity.Property(e => e.Iso3)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ISO3");
            entity.Property(e => e.ModifiedOn).HasPrecision(0);
            entity.Property(e => e.PhoneCode).HasMaxLength(20);
        });

        modelBuilder.Entity<District>(entity =>
        {
            entity.HasIndex(e => e.StateId, "IX_Districts_StateId");

            entity.HasIndex(e => new { e.StateId, e.DistrictCode }, "UQ_Districts_State_DistrictCode").IsUnique();

            entity.Property(e => e.CreatedOn)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_Districts_CreatedOn");
            entity.Property(e => e.DistrictCode).HasMaxLength(100);
            entity.Property(e => e.DistrictName).HasMaxLength(200);
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Districts_IsActive");
            entity.Property(e => e.ModifiedOn).HasPrecision(0);

            entity.HasOne(d => d.State).WithMany(p => p.Districts)
                .HasForeignKey(d => d.StateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Districts_States");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(e => e.RoleName, "UQ_Roles_RoleName").IsUnique();

            entity.Property(e => e.CreatedOn)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_Roles_CreatedOn");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Roles_IsActive");
            entity.Property(e => e.ModifiedOn).HasPrecision(0);
            entity.Property(e => e.RoleName).HasMaxLength(30);
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.HasIndex(e => e.CountryId, "IX_States_CountryId");

            entity.HasIndex(e => new { e.CountryId, e.StateCode }, "UQ_States_Country_StateCode").IsUnique();

            entity.Property(e => e.CreatedOn)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_States_CreatedOn");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_States_IsActive");
            entity.Property(e => e.ModifiedOn).HasPrecision(0);
            entity.Property(e => e.StateCode).HasMaxLength(30);
            entity.Property(e => e.StateName).HasMaxLength(200);

            entity.HasOne(d => d.Country).WithMany(p => p.States)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_States_Countries");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_Users_RoleId");

            entity.HasIndex(e => e.Email, "UQ_Users_Email").IsUnique();

            entity.HasIndex(e => e.PhoneNumber, "UX_Users_PhoneNumber")
                .IsUnique()
                .HasFilter("([PhoneNumber] IS NOT NULL)");

            entity.Property(e => e.CreatedOn)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_Users_CreatedOn");
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.FullAddress).HasMaxLength(500);
            entity.Property(e => e.FullName).HasMaxLength(150);
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Users_IsActive");
            entity.Property(e => e.ModifiedOn).HasPrecision(0);
            entity.Property(e => e.PasswordHash).HasMaxLength(500);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
