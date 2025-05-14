using Microsoft.EntityFrameworkCore;
using ParkingReservation.Models;

namespace ParkingReservation.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<ReservationType> ReservationTypes { get; set; }

    public virtual DbSet<Space> Spaces { get; set; }

    public virtual DbSet<State> States { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5435;Database=parking_reservation;Username=root;Password=1234");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("admins_pkey");

            entity.ToTable("admins");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AddedAt)
                .HasPrecision(2)
                .HasDefaultValueSql("now()")
                .HasColumnName("added_at");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("reservations_pkey");

            entity.ToTable("reservations");

            entity.HasIndex(e => e.SpaceNumber, "index_spn_reservations");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.BeginsAt)
                .HasPrecision(0)
                .HasColumnName("begins_at");
            entity.Property(e => e.Comment)
                .HasMaxLength(256)
                .HasColumnName("comment");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(2)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.EndsAt)
                .HasPrecision(0)
                .HasColumnName("ends_at");
            entity.Property(e => e.SpaceNumber).HasColumnName("space_number");
            entity.Property(e => e.StateId)
                .HasDefaultValue(1)
                .HasColumnName("state_id");
            entity.Property(e => e.TypeId).HasColumnName("type_id");
            entity.Property(e => e.UserId)
                .HasMaxLength(64)
                .HasColumnName("user_id");

            entity.HasOne(d => d.SpaceNumberNavigation).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.SpaceNumber)
                .HasConstraintName("reservations_space_number_fkey");

            entity.HasOne(d => d.State).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.StateId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("reservations_state_id_fkey");

            entity.HasOne(d => d.Type).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("reservations_type_id_fkey");
        });

        modelBuilder.Entity<ReservationType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("reservation_types_pkey");

            entity.ToTable("reservation_types");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Space>(entity =>
        {
            entity.HasKey(e => e.SpaceNumber).HasName("spaces_pkey");

            entity.ToTable("spaces");

            entity.Property(e => e.SpaceNumber).HasColumnName("space_number");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(2)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(64)
                .HasColumnName("created_by");
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("states_pkey");

            entity.ToTable("states");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .HasColumnName("name");
            entity.Property(e => e.NameCs)
                .HasMaxLength(64)
                .HasColumnName("name_cs");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
