using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Schedule_Management.Models;

public partial class ScheduleManagementDbContext : DbContext
{
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActivityType>(entity =>
        {
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())", "DF_ActivityTypes_CreatedOn");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_ActivityTypes_IsActive");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.Property(e => e.BookedOn).HasDefaultValueSql("(sysutcdatetime())", "DF_Bookings_BookedOn");
            entity.Property(e => e.BookingStatus).HasDefaultValue("Confirmed", "DF_Bookings_BookingStatus");
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())", "DF_Bookings_CreatedOn");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Bookings_IsActive");

            entity.HasOne(d => d.Availability).WithMany(p => p.Bookings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bookings_CoachAvailabilities");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bookings_Users");
        });

        modelBuilder.Entity<CoachAvailability>(entity =>
        {
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())", "DF_CoachAvailabilities_CreatedOn");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_CoachAvailabilities_IsActive");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.ActivityType).WithMany(p => p.CoachAvailabilities)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CoachAvailabilities_ActivityTypes");

            entity.HasOne(d => d.Coach).WithMany(p => p.CoachAvailabilities)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CoachAvailabilities_Users");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())", "DF_Countries_CreatedOn");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Countries_IsActive");
            entity.Property(e => e.Iso2).IsFixedLength();
            entity.Property(e => e.Iso3).IsFixedLength();
        });

        modelBuilder.Entity<District>(entity =>
        {
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())", "DF_Districts_CreatedOn");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Districts_IsActive");

            entity.HasOne(d => d.State).WithMany(p => p.Districts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Districts_States");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())", "DF_Roles_CreatedOn");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Roles_IsActive");
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())", "DF_States_CreatedOn");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_States_IsActive");

            entity.HasOne(d => d.Country).WithMany(p => p.States)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_States_Countries");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.PhoneNumber, "UX_Users_PhoneNumber")
                .IsUnique()
                .HasFilter("([PhoneNumber] IS NOT NULL)");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())", "DF_Users_CreatedOn");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Users_IsActive");

            entity.HasOne(d => d.District).WithMany(p => p.Users).HasConstraintName("FK_Users_Districts");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
