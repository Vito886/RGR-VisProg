using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.IO;

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

        private string DbPath = @"Assets\DogRun.db";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string directoryPath = Directory.GetCurrentDirectory();
                directoryPath = directoryPath.Remove(directoryPath.LastIndexOf("bin"));
                DbPath = directoryPath + DbPath;
                optionsBuilder.UseSqlite("Data source=" + DbPath);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dog>(entity =>
            {
                entity.HasKey(e => e.Name);

                entity.Property(e => e.OwnerName).HasColumnName("Owner_name");

                entity.Property(e => e.TrainerName).HasColumnName("Trainer_name");

                entity.HasOne(d => d.OwnerNameNavigation)
                    .WithMany(p => p.DogOwnerNameNavigations)
                    .HasForeignKey(d => d.OwnerName)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.TrainerNameNavigation)
                    .WithMany(p => p.DogTrainerNameNavigations)
                    .HasForeignKey(d => d.TrainerName)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Owner>(entity =>
            {
                entity.HasKey(e => e.Name);
            });

            modelBuilder.Entity<Result>(entity =>
            {
                entity.HasKey(e => new { e.RunId, e.DogName });

                entity.Property(e => e.RunId).HasColumnName("Run_ID");

                entity.Property(e => e.DogName).HasColumnName("Dog_name");

                entity.HasOne(d => d.DogNameNavigation)
                    .WithMany(p => p.Results)
                    .HasForeignKey(d => d.DogName)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Run)
                    .WithMany(p => p.Results)
                    .HasForeignKey(d => d.RunId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Run>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");
            });

            modelBuilder.Entity<Trainer>(entity =>
            {
                entity.HasKey(e => e.Name);

                entity.Property(e => e.BestDog).HasColumnName("Best_dog");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
