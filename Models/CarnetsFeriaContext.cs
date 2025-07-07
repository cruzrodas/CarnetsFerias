using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CarnetsdeFeria.Models;

public partial class CarnetsFeriaContext : DbContext
{
    public CarnetsFeriaContext()
    {
    }

    public CarnetsFeriaContext(DbContextOptions<CarnetsFeriaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DocumentoIdentificacion> DocumentoIdentificacions { get; set; }

    public virtual DbSet<EspacioParque> EspacioParques { get; set; }

    public virtual DbSet<FeriaArea> FeriaAreas { get; set; }

    public virtual DbSet<FeriaCarnet> FeriaCarnets { get; set; }

    public virtual DbSet<Ferium> Feria { get; set; }

    public virtual DbSet<Participacion> Participacions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DocumentoIdentificacion>(entity =>
        {
            entity.ToTable("DocumentoIdentificacion");

            entity.Property(e => e.TipoIdentificacion)
                .HasMaxLength(50)
                .IsFixedLength();
        });

        modelBuilder.Entity<EspacioParque>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_VentasEspacioParque");

            entity.ToTable("EspacioParque");

            entity.Property(e => e.DescripcionEspacio)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Informacion).IsUnicode(false);
        });

        modelBuilder.Entity<FeriaArea>(entity =>
        {
            entity.ToTable("FeriaArea");

            entity.Property(e => e.CantidadStand)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.DescripcionArea)
                .HasMaxLength(100)
                .IsFixedLength();

            entity.HasOne(d => d.IdEspacioNavigation).WithMany(p => p.FeriaAreas)
                .HasForeignKey(d => d.IdEspacio)
                .HasConstraintName("FK_FeriaArea_VentasEspacioParque");

            entity.HasOne(d => d.IdFeriaNavigation).WithMany(p => p.FeriaAreas)
                .HasForeignKey(d => d.IdFeria)
                .HasConstraintName("FK_FeriaArea_Feria");
        });

        modelBuilder.Entity<FeriaCarnet>(entity =>
        {
            entity.ToTable("FeriaCarnet");

            entity.Property(e => e.Apellido)
                .HasMaxLength(50)
                .IsFixedLength();
            entity.Property(e => e.BoletaCortesia).HasColumnName("Boleta_Cortesia");
            entity.Property(e => e.FechaCreacion).HasColumnName("Fecha_Creacion");
            entity.Property(e => e.FechaModificacion).HasColumnName("Fecha_Modificacion");
            entity.Property(e => e.Foto).IsUnicode(false);
            entity.Property(e => e.IdDocIdentificacion).HasColumnName("Id_DocIdentificacion");
            entity.Property(e => e.IdParticipacion).HasColumnName("Id_participacion");
            entity.Property(e => e.NoBoleta).HasColumnName("No_Boleta");
            entity.Property(e => e.NoIdenitficacion).HasColumnName("No_Idenitficacion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsFixedLength();

            entity.HasOne(d => d.IdDocIdentificacionNavigation).WithMany(p => p.FeriaCarnets)
                .HasForeignKey(d => d.IdDocIdentificacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FeriaCarnet_DocumentoIdentificacion");

            entity.HasOne(d => d.IdFeriaAreaNavigation).WithMany(p => p.FeriaCarnets)
                .HasForeignKey(d => d.IdFeriaArea)
                .HasConstraintName("FK_FeriaCarnet_FeriaArea");

            entity.HasOne(d => d.IdParticipacionNavigation).WithMany(p => p.FeriaCarnets)
                .HasForeignKey(d => d.IdParticipacion)
                .HasConstraintName("FK_FeriaCarnet_Participacion");
        });

        modelBuilder.Entity<Ferium>(entity =>
        {
            entity.Property(e => e.DescripcionFeria)
                .HasMaxLength(100)
                .IsFixedLength();
        });

        modelBuilder.Entity<Participacion>(entity =>
        {
            entity.ToTable("Participacion");

            entity.Property(e => e.DescripcionParticipacion)
                .HasMaxLength(50)
                .IsFixedLength();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
