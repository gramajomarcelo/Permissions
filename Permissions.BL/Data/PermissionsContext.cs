using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Permissions.BL.Models;

namespace Permissions.BL.Data;

public partial class PermissionsContext : DbContext
{

    public PermissionsContext(DbContextOptions<PermissionsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<PermissionType> PermissionTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Permissi__3214EC07EAA9998F");

            entity.ToTable("Permission");

            entity.Property(e => e.EmployeeForename).HasMaxLength(50);
            entity.Property(e => e.EmployeeSurname).HasMaxLength(50);
            entity.Property(e => e.PermissionDate).HasColumnType("date");

            entity.HasOne(d => d.PermissionTypeNavigation).WithMany(p => p.Permissions)
                .HasForeignKey(d => d.PermissionType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Permissio__Permi__398D8EEE");
        });

        modelBuilder.Entity<PermissionType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Permissi__3214EC07C3B00DBB");

            entity.ToTable("PermissionType");

            entity.Property(e => e.Description).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
