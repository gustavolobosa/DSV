using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace verificable.Models;

public partial class BbddverificableContext : DbContext
{
    public BbddverificableContext()
    {
    }

    public BbddverificableContext(DbContextOptions<BbddverificableContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Adquirente> Adquirentes { get; set; }

    public virtual DbSet<Enajenante> Enajenantes { get; set; }

    public virtual DbSet<Formulario> Formularios { get; set; }

    public virtual DbSet<Multipropietario> Multipropietarios { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
    //    => optionsBuilder.UseSqlServer("server=LAPTOP-DE-GUSTA; database=BBDDVerificable; integrated security=true; encrypt=false");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Adquirente>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__adquiren__3213E83FCEE905D5");

            entity.ToTable("adquirentes");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NoAcreditado).HasColumnName("no_acreditado");
            entity.Property(e => e.NumAtencion).HasColumnName("num_atencion");
            entity.Property(e => e.PorcentajeDerecho).HasColumnName("porcentaje_derecho");
            entity.Property(e => e.RunRut)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("run_rut");

            entity.HasOne(d => d.NumAtencionNavigation).WithMany(p => p.Adquirentes)
                .HasForeignKey(d => d.NumAtencion)
                .HasConstraintName("FK__adquirent__num_a__29572725");
        });

        modelBuilder.Entity<Enajenante>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__enajenan__3213E83F70A44ADC");

            entity.ToTable("enajenantes");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NoAcreditado).HasColumnName("no_acreditado");
            entity.Property(e => e.NumAtencion).HasColumnName("num_atencion");
            entity.Property(e => e.PorcentajeDerecho).HasColumnName("porcentaje_derecho");
            entity.Property(e => e.RunRut)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("run_rut");

            entity.HasOne(d => d.NumAtencionNavigation).WithMany(p => p.Enajenantes)
                .HasForeignKey(d => d.NumAtencion)
                .HasConstraintName("FK__enajenant__num_a__267ABA7A");
        });

        modelBuilder.Entity<Formulario>(entity =>
        {
            entity.HasKey(e => e.NumAtencion).HasName("PK__formular__0B04B3918F45318E");

            entity.ToTable("formularios");

            entity.Property(e => e.NumAtencion).HasColumnName("num_atencion");
            entity.Property(e => e.Cne)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cne");
            entity.Property(e => e.Comuna)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("comuna");
            entity.Property(e => e.FechaInscripcion)
                .HasColumnType("date")
                .HasColumnName("fecha_inscripcion");
            entity.Property(e => e.Fojas).HasColumnName("fojas");
            entity.Property(e => e.Manzana)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("manzana");
            entity.Property(e => e.NumInscripcion).HasColumnName("num_inscripcion");
            entity.Property(e => e.Predio)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("predio");
        });

        modelBuilder.Entity<Multipropietario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__multipro__3213E83F4500D453");

            entity.ToTable("multipropietarios");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Comuna)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("comuna");
            entity.Property(e => e.FechaInscripcion)
                .HasColumnType("date")
                .HasColumnName("fecha_inscripcion");
            entity.Property(e => e.Fojas).HasColumnName("fojas");
            entity.Property(e => e.Manzana)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("manzana");
            entity.Property(e => e.NumInscripcion).HasColumnName("num_inscripcion");
            entity.Property(e => e.PorcentajeDerecho).HasColumnName("porcentaje_derecho");
            entity.Property(e => e.Predio)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("predio");
            entity.Property(e => e.RunRut)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("run_rut");
            entity.Property(e => e.VigenciaFinal)
                .HasColumnType("date")
                .HasColumnName("vigencia_final");
            entity.Property(e => e.VigenciaInicial)
                .HasColumnType("date")
                .HasColumnName("vigencia_inicial");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
