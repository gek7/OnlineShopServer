using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace OnlineShopServerCore.Models
{
    public partial class OnlineShopContext : DbContext
    {
        public OnlineShopContext()
        {
        }

        public OnlineShopContext(DbContextOptions<OnlineShopContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<CategoryAttribute> CategoryAttributes { get; set; }
        public virtual DbSet<CategoryAttributesType> CategoryAttributesTypes { get; set; }
        public virtual DbSet<EnumCategoryAttributesValue> EnumCategoryAttributesValues { get; set; }
        public virtual DbSet<Filter> Filters { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<ItemAttribute> ItemAttributes { get; set; }
        public virtual DbSet<ItemAttributesValue> ItemAttributesValues { get; set; }
        public virtual DbSet<ItemImage> ItemImages { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<OrderStatus> OrderStatuses { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Unit> Units { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<WishList> WishLists { get; set; }

        /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=.\\GEKPC;Initial Catalog=OnlineShop;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            }
        }*/

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Cyrillic_General_CI_AS");

            modelBuilder.Entity<Cart>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.Carts)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Carts_Items");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Carts)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Carts_Users");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasOne(d => d.OwnerNavigation)
                    .WithMany(p => p.InverseOwnerNavigation)
                    .HasForeignKey(d => d.Owner)
                    .HasConstraintName("FK_Categories_Categories");
            });

            modelBuilder.Entity<CategoryAttribute>(entity =>
            {
                entity.HasOne(d => d.Category)
                    .WithMany(p => p.CategoryAttributes)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_CategoryAttributes_Categories");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.CategoryAttributes)
                    .HasForeignKey(d => d.TypeId)
                    .HasConstraintName("FK_CategoryAttributes_CategoryAttributesType");
            });

            modelBuilder.Entity<CategoryAttributesType>(entity =>
            {
                entity.ToTable("CategoryAttributesType");
            });

            modelBuilder.Entity<EnumCategoryAttributesValue>(entity =>
            {
                entity.ToTable("EnumCategoryAttributesValue");

                entity.HasOne(d => d.CategoryAttributes)
                    .WithMany(p => p.EnumCategoryAttributesValues)
                    .HasForeignKey(d => d.CategoryAttributesId)
                    .HasConstraintName("FK_EnumCategoryAttributesValue_CategoryAttributes");
            });

            modelBuilder.Entity<Filter>(entity =>
            {
                entity.ToTable("Filter");

                entity.Property(e => e.IsEnabled).HasColumnName("isEnabled");

                entity.HasOne(d => d.CategoryAttribute)
                    .WithMany(p => p.Filters)
                    .HasForeignKey(d => d.CategoryAttributeId)
                    .HasConstraintName("FK_Filter_CategoryAttributes");
            });

            modelBuilder.Entity<Item>(entity =>
            {
                entity.Property(e => e.Price).HasColumnType("money");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Items)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_Items_Categories");

                entity.HasOne(d => d.OwnerNavigation)
                    .WithMany(p => p.InverseOwnerNavigation)
                    .HasForeignKey(d => d.Owner)
                    .HasConstraintName("FK_Items_Items");
            });

            modelBuilder.Entity<ItemAttribute>(entity =>
            {
                entity.HasOne(d => d.CategoryAttributes)
                    .WithMany(p => p.ItemAttributes)
                    .HasForeignKey(d => d.CategoryAttributesId)
                    .HasConstraintName("FK_ItemAttributes_CategoryAttributes");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.ItemAttributes)
                    .HasForeignKey(d => d.ItemId)
                    .HasConstraintName("FK_ItemAttributes_Items");
            });

            modelBuilder.Entity<ItemAttributesValue>(entity =>
            {
                entity.HasOne(d => d.ItemAttribute)
                    .WithMany(p => p.ItemAttributesValues)
                    .HasForeignKey(d => d.ItemAttributeId)
                    .HasConstraintName("FK_ItemAttributesValues_ItemAttributes");

                entity.HasOne(d => d.Unit)
                    .WithMany(p => p.ItemAttributesValues)
                    .HasForeignKey(d => d.UnitId)
                    .HasConstraintName("FK_ItemAttributesValues_Units");
            });

            modelBuilder.Entity<ItemImage>(entity =>
            {
                entity.HasOne(d => d.Item)
                    .WithMany(p => p.ItemImages)
                    .HasForeignKey(d => d.ItemId)
                    .HasConstraintName("FK_ItemImages_Items");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.PhoneNumber).HasMaxLength(50);

                entity.HasOne(d => d.OrderStatus)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.OrderStatusId)
                    .HasConstraintName("FK_Orders_OrderStatuses");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Orders_Users");
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.Property(e => e.Price).HasColumnType("money");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.ItemId)
                    .HasConstraintName("FK_OrderItems_Items");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_OrderItems_Orders");
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasOne(d => d.Item)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.ItemId)
                    .HasConstraintName("FK_Reviews_Items");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Reviews_Users");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.LastLoginDate).HasColumnType("date");

                entity.Property(e => e.RegisterDate).HasColumnType("date");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_Users_Roles");
            });

            modelBuilder.Entity<WishList>(entity =>
            {
                entity.ToTable("WishList");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.WishLists)
                    .HasForeignKey(d => d.ItemId)
                    .HasConstraintName("FK_WishList_Items");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.WishLists)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_WishList_Users");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
