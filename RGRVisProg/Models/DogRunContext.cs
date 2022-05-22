using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace RGRVisProg.Models
{
    public partial class DogRunContext : DbContext
    {
        public DogRunContext()
        {
        }

        public DogRunContext(DbContextOptions<DogRunContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Dog> Dogs { get; set; } = null!;
        public virtual DbSet<Owner> Owners { get; set; } = null!;
        public virtual DbSet<Result> Results { get; set; } = null!;
        public virtual DbSet<Run> Runs { get; set; } = null!;
        public virtual DbSet<Trainer> Trainers { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=C:\\Users\\nikit\\source\\repos\\RGR-VisProg\\RGRVisProg\\Assets\\DogRun.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dog>(entity =>
            {
                entity.HasKey(e => e.Name);

                entity.ToTable("Dog");

                entity.Property(e => e.Name).HasColumnType("STRING");

                entity.Property(e => e.OwnerName)
                    .HasColumnType("STRING")
                    .HasColumnName("Owner name");

                entity.Property(e => e.P).HasColumnName("P%");

                entity.Property(e => e.TrainerName)
                    .HasColumnType("STRING")
                    .HasColumnName("Trainer name");

                entity.Property(e => e.W).HasColumnName("W%");

                entity.Property(e => e._2nds).HasColumnName("2nds");

                entity.Property(e => e._3rds).HasColumnName("3rds");

                entity.HasOne(d => d.OwnerNameNavigation)
                    .WithMany(p => p.Dogs)
                    .HasForeignKey(d => d.OwnerName);

                entity.HasOne(d => d.TrainerNameNavigation)
                    .WithMany(p => p.Dogs)
                    .HasForeignKey(d => d.TrainerName);
            });

            modelBuilder.Entity<Owner>(entity =>
            {
                entity.HasKey(e => e.Name);

                entity.ToTable("Owner");

                entity.Property(e => e.Name).HasColumnType("STRING");
            });

            modelBuilder.Entity<Result>(entity =>
            {
                entity.HasKey(e => new { e.RunId, e.DogName });

                entity.ToTable("Result");

                entity.Property(e => e.RunId).HasColumnName("Run ID");

                entity.Property(e => e.DogName)
                    .HasColumnType("STRING")
                    .HasColumnName("Dog name");

                entity.Property(e => e.Time).HasColumnType("DATETIME");

                entity.HasOne(d => d.Run)
                    .WithMany(p => p.Results)
                    .HasForeignKey(d => d.RunId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Run>(entity =>
            {
                entity.ToTable("Run");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.Date).HasColumnType("DATETIME");

                entity.Property(e => e.Track).HasColumnType("STRING");
            });

            modelBuilder.Entity<Trainer>(entity =>
            {
                entity.HasKey(e => e.Name);

                entity.ToTable("Trainer");

                entity.Property(e => e.BestDog).HasColumnName("Best dog");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
