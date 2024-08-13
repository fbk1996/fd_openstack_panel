using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using acl_openstack_identity.Models;

namespace acl_openstack_identity.Data;

public partial class OpenstackContext : DbContext
{
    public OpenstackContext()
    {
    }

    public OpenstackContext(DbContextOptions<OpenstackContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ApplicationCredential> ApplicationCredentials { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectsUser> ProjectsUsers { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UsersRole> UsersRoles { get; set; }

    public virtual DbSet<UsersToken> UsersTokens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Name=ConnectionStrings:OpenstackDatabase");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationCredential>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("application_credentials_pk");

            entity.ToTable("application_credentials");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Expire)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("expire");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Secret)
                .HasMaxLength(80)
                .HasColumnName("secret");
            entity.Property(e => e.UserId)
                .ValueGeneratedOnAdd()
                .HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.ApplicationCredentials)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("application_credentials_users_id_fk");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("projects_pk");

            entity.ToTable("projects");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("creation_date");
            entity.Property(e => e.DescriptionText).HasColumnName("description_text");
            entity.Property(e => e.Groupid)
                .HasMaxLength(40)
                .HasColumnName("groupid");
            entity.Property(e => e.Metadata)
                .HasColumnType("jsonb")
                .HasColumnName("metadata");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.OpenstackProjectId)
                .HasMaxLength(36)
                .HasColumnName("openstack_project_id");
            entity.Property(e => e.ParentId)
                .ValueGeneratedOnAdd()
                .HasColumnName("parent_id");
            entity.Property(e => e.Scope).HasColumnName("scope");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("projects_projects_id_fk");
        });

        modelBuilder.Entity<ProjectsUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("projects_users_pk");

            entity.ToTable("projects_users");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ProjectId)
                .ValueGeneratedOnAdd()
                .HasColumnName("project_id");
            entity.Property(e => e.UserId)
                .ValueGeneratedOnAdd()
                .HasColumnName("user_id");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectsUsers)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("projects_users_projects_id_fk");

            entity.HasOne(d => d.User).WithMany(p => p.ProjectsUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("projects_users_users_id_fk");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(60)
                .HasColumnName("name");
            entity.Property(e => e.Permissions).HasColumnName("permissions");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.Scope).HasColumnName("scope");

            entity.HasOne(d => d.Project).WithMany(p => p.Roles)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("roles_projects_id_fk");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Icon).HasColumnName("icon");
            entity.Property(e => e.Isfirstlogin)
                .HasDefaultValueSql("false")
                .HasColumnName("isfirstlogin");
            entity.Property(e => e.Lastname)
                .HasMaxLength(25)
                .HasColumnName("lastname");
            entity.Property(e => e.Name)
                .HasMaxLength(25)
                .HasColumnName("name");
            entity.Property(e => e.OpenstackId)
                .HasMaxLength(40)
                .HasColumnName("openstack_id");
            entity.Property(e => e.Openstackpassword).HasColumnName("openstackpassword");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
        });

        modelBuilder.Entity<UsersRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_roles_pkey");

            entity.ToTable("users_roles");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.RoleId)
                .ValueGeneratedOnAdd()
                .HasColumnName("role_id");
            entity.Property(e => e.UserId)
                .ValueGeneratedOnAdd()
                .HasColumnName("user_id");

            entity.HasOne(d => d.Role).WithMany(p => p.UsersRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_roles_roles_id_fk");

            entity.HasOne(d => d.User).WithMany(p => p.UsersRoles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_roles_users_id_fk");
        });

        modelBuilder.Entity<UsersToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_tokens_pk");

            entity.ToTable("users_tokens");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Expire)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("expire");
            entity.Property(e => e.Token)
                .HasMaxLength(25)
                .HasColumnName("token");
            entity.Property(e => e.UserId)
                .ValueGeneratedOnAdd()
                .HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.UsersTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_tokens_users_id_fk");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
