using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Repository.Models;
using System;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace RepositoryLayer.Repository
{
    public partial class scaffoldingDbContext : DbContext
    {
        public scaffoldingDbContext()
        {
        }

        public scaffoldingDbContext(DbContextOptions<scaffoldingDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<UserNotes> Notes { get; set; }
        public virtual DbSet<Persons> Persons { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("SqlConnection"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserNotes>(entity =>
            {
                entity.HasKey(e => e.NoteId)
                    .HasName("PK__Notes__EACE355FB81BDD6E");

                entity.Property(e => e.Color)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('white')");

                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.IsArchive).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.Title).IsUnicode(false);

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.Notes)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Notes__PersonID__07C12930");
            });

            modelBuilder.Entity<Persons>(entity =>
            {
                entity.Property(e => e.Email).IsUnicode(false);

                entity.Property(e => e.FirstName).IsUnicode(false);

                entity.Property(e => e.LastName).IsUnicode(false);

                entity.Property(e => e.Password).IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
