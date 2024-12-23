using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SatTrack.DAL;

public partial class ElderveilContext : DbContext
{
    public ElderveilContext()
    {
    }

    public ElderveilContext(DbContextOptions<ElderveilContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Sat> Sats { get; set; }

    public virtual DbSet<SatGroup> SatGroups { get; set; }

    public virtual DbSet<SatGroupsCat> SatGroupsCats { get; set; }

    public virtual DbSet<User> Users { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("roles_pk");

            entity.Property(e => e.RoleId).UseIdentityAlwaysColumn();
        });

        modelBuilder.Entity<Sat>(entity =>
        {
            entity.HasKey(e => e.SatId).HasName("sats_pk");

            entity.Property(e => e.SatId).UseIdentityAlwaysColumn();

            entity.HasMany(d => d.SatGroups).WithMany(p => p.Sats)
                .UsingEntity<Dictionary<string, object>>(
                    "SatGroupAssociation",
                    r => r.HasOne<SatGroup>().WithMany()
                        .HasForeignKey("SatGroupId")
                        .HasConstraintName("sat_group_association_sat_groups_sat_group_id_fk"),
                    l => l.HasOne<Sat>().WithMany()
                        .HasForeignKey("SatId")
                        .HasConstraintName("sat_group_association_sats_sat_id_fk"),
                    j =>
                    {
                        j.HasKey("SatId", "SatGroupId").HasName("sat_group_association_pk");
                        j.ToTable("sat_group_association", "sattracker");
                        j.IndexerProperty<int>("SatId").HasColumnName("sat_id");
                        j.IndexerProperty<int>("SatGroupId").HasColumnName("sat_group_id");
                    });
        });

        modelBuilder.Entity<SatGroup>(entity =>
        {
            entity.HasKey(e => e.SatGroupId).HasName("sat_groups_pk");

            entity.Property(e => e.SatGroupId).UseIdentityAlwaysColumn();

            entity.HasOne(d => d.SatGroupCat).WithMany(p => p.SatGroups)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("sat_groups_sat_groups_cats_sat_group_cat_id_fk");
        });

        modelBuilder.Entity<SatGroupsCat>(entity =>
        {
            entity.HasKey(e => e.SatGroupCatId).HasName("sat_groups_cats_pk");

            entity.Property(e => e.SatGroupCatId).UseIdentityAlwaysColumn();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pk");

            entity.Property(e => e.UserId).UseIdentityAlwaysColumn();
            entity.Property(e => e.Active).HasDefaultValue(true);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("user_roles_roles_role_id_fk"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("user_roles_users_user_id_fk"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId").HasName("user_roles_pk");
                        j.ToTable("user_roles", "sattracker");
                        j.IndexerProperty<int>("UserId").HasColumnName("user_id");
                        j.IndexerProperty<int>("RoleId").HasColumnName("role_id");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
