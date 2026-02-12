using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace StoreManagementService.Models
{
    public partial class StoreManagementContext : DbContext
    {
        public StoreManagementContext()
        {
        }

        public StoreManagementContext(DbContextOptions<StoreManagementContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<ItemsClientsRelationship> ItemsClientsRelationships { get; set; }
        public virtual DbSet<ItemsStoresRelationship> ItemsStoresRelationships { get; set; }
        public virtual DbSet<OperationLog> OperationLogs { get; set; }
        public virtual DbSet<SessionLog> SessionLogs { get; set; }
        public virtual DbSet<StatusCode> StatusCodes { get; set; }
        public virtual DbSet<Store> Stores { get; set; }
        public virtual DbSet<VerifyCode> VerifyCodes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // get connection string from environment variable
                string ConnectionStrings = System.Environment.GetEnvironmentVariable("ConnectionStrings");

                optionsBuilder.UseSqlServer(ConnectionStrings);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Modern_Spanish_CI_AS");

            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasIndex(e => e.EmailAddress, "UQ__Clients__347C3027DF1F2C2C")
                    .IsUnique();

                entity.Property(e => e.ClientId)
                    .ValueGeneratedNever()
                    .HasColumnName("ClientID");

                entity.Property(e => e.ClientAddress).IsRequired();

                entity.Property(e => e.ClientLastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.ClientName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.EmailAddress)
                    .HasMaxLength(50)
                    .HasColumnName("emailAddress");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("isDeleted")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.PasswordHash).IsRequired();

                entity.Property(e => e.PasswordSalst).IsRequired();
            });

            modelBuilder.Entity<Item>(entity =>
            {
                entity.Property(e => e.ItemId)
                    .ValueGeneratedNever()
                    .HasColumnName("ItemID");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("isDeleted")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ItemCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ItemDescription)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ItemImg)
                    .IsRequired()
                    .HasColumnType("image");

                entity.Property(e => e.ItemPrice).HasColumnType("decimal(10, 2)");
            });

            modelBuilder.Entity<ItemsClientsRelationship>(entity =>
            {
                entity.ToTable("ItemsClientsRelationship");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.ClientId).HasColumnName("ClientID");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("isDeleted")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ItemId).HasColumnName("ItemID");

                entity.Property(e => e.OperationDate).HasColumnType("datetime");

                entity.Property(e => e.WasSold).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.ItemsClientsRelationships)
                    .HasForeignKey(d => d.ClientId)
                    .HasConstraintName("FK__ItemsClie__Clien__32E0915F");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.ItemsClientsRelationships)
                    .HasForeignKey(d => d.ItemId)
                    .HasConstraintName("FK__ItemsClie__ItemI__33D4B598");
            });

            modelBuilder.Entity<ItemsStoresRelationship>(entity =>
            {
                entity.ToTable("ItemsStoresRelationship");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("isDeleted")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ItemId).HasColumnName("ItemID");

                entity.Property(e => e.OperationDate).HasColumnType("datetime");

                entity.Property(e => e.StoreId).HasColumnName("StoreID");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.ItemsStoresRelationships)
                    .HasForeignKey(d => d.ItemId)
                    .HasConstraintName("FK__ItemsStor__ItemI__2E1BDC42");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.ItemsStoresRelationships)
                    .HasForeignKey(d => d.StoreId)
                    .HasConstraintName("FK__ItemsStor__Store__2F10007B");
            });

            modelBuilder.Entity<OperationLog>(entity =>
            {
                entity.HasKey(e => e.OperationId)
                    .HasName("PK__Operatio__A4F5FC64AE66DE3E");

                entity.ToTable("OperationLog");

                entity.Property(e => e.OperationId).HasColumnName("OperationID");

                entity.Property(e => e.OperationDate).HasColumnType("datetime");

                entity.Property(e => e.SessionId).HasColumnName("SessionID");

                entity.HasOne(d => d.Session)
                    .WithMany(p => p.OperationLogs)
                    .HasForeignKey(d => d.SessionId)
                    .HasConstraintName("FK__Operation__Respo__3B75D760");
            });

            modelBuilder.Entity<SessionLog>(entity =>
            {
                entity.HasKey(e => e.SessionId)
                    .HasName("PK__SessionL__C9F49270A486AE83");

                entity.ToTable("SessionLog");

                entity.Property(e => e.SessionId)
                    .ValueGeneratedNever()
                    .HasColumnName("SessionID");

                entity.Property(e => e.ClientId).HasColumnName("ClientID");

                entity.Property(e => e.EndSession).HasColumnType("datetime");

                entity.Property(e => e.InitSession).HasColumnType("datetime");

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.SessionLogs)
                    .HasForeignKey(d => d.ClientId)
                    .HasConstraintName("FK__SessionLo__EndSe__38996AB5");
            });

            modelBuilder.Entity<StatusCode>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.StatusDescription)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Store>(entity =>
            {
                entity.Property(e => e.StoreId)
                    .ValueGeneratedNever()
                    .HasColumnName("StoreID");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("isDeleted")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.StoreAddress)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.StoreBranch)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<VerifyCode>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.ClientId).HasColumnName("ClientID");

                entity.Property(e => e.Code).HasMaxLength(15);

                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.Property(e => e.VerifyDate).HasColumnType("datetime");

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.VerifyCodes)
                    .HasForeignKey(d => d.ClientId)
                    .HasConstraintName("FK__VerifyCod__Verif__403A8C7D");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
