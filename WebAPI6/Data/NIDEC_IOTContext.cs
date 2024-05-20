using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WebAPI6.Data
{
    public partial class NIDEC_IOTContext : IdentityDbContext<ApplicationUser>
    {
        public NIDEC_IOTContext()
        {
        }

        public NIDEC_IOTContext(DbContextOptions<NIDEC_IOTContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Book> Books { get; set; } = null!;
        public virtual DbSet<TIotMoldMaster> TIotMoldMasters { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=LAPTOP-99421S3D\\SQLEXPRESS;Initial Catalog=NIDEC_IOT;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(p => p.UserId);
            modelBuilder.Entity<Book>(entity =>
            {
                entity.ToTable("Book");

                entity.Property(e => e.Title).HasMaxLength(100);
            });

            modelBuilder.Entity<TIotMoldMaster>(entity =>
            {
                entity.HasKey(e => e.MoldSerial)
                    .HasName("PK_T_IOT_MOLD_MASTER_MOLD_SERIAL");

                entity.ToTable("T_IOT_MOLD_MASTER");

                entity.Property(e => e.MoldSerial)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("mold_serial");

                entity.Property(e => e.CavQty)
                    .HasColumnType("numeric(18, 0)")
                    .HasColumnName("cav_qty");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.MachineCd)
                    .HasMaxLength(50)
                    .HasColumnName("machine_cd");

                entity.Property(e => e.MachineStatus).HasColumnName("machine_status");

                entity.Property(e => e.MaintenanceQty)
                    .HasColumnType("numeric(18, 0)")
                    .HasColumnName("maintenance_qty");

                entity.Property(e => e.MaintenanceShot)
                    .HasColumnType("numeric(18, 0)")
                    .HasColumnName("maintenance_shot");

                entity.Property(e => e.MaterialType)
                    .HasMaxLength(50)
                    .HasColumnName("material_type");

                entity.Property(e => e.MaterialUsage)
                    .HasMaxLength(50)
                    .HasColumnName("material_usage");

                entity.Property(e => e.MoldName)
                    .HasMaxLength(50)
                    .HasColumnName("mold_name");

                entity.Property(e => e.MoldNo)
                    .HasMaxLength(50)
                    .HasColumnName("mold_no");

                entity.Property(e => e.ReycleRatio)
                    .HasColumnType("numeric(18, 0)")
                    .HasColumnName("reycle_ratio");

                entity.Property(e => e.ScrapQty)
                    .HasColumnType("numeric(18, 0)")
                    .HasColumnName("scrap_qty");

                entity.Property(e => e.ScrapShot)
                    .HasColumnType("numeric(18, 0)")
                    .HasColumnName("scrap_shot");

                entity.Property(e => e.StackQty)
                    .HasColumnType("numeric(18, 0)")
                    .HasColumnName("stack_qty");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
