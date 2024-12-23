using Microsoft.EntityFrameworkCore;
using Web.Models;

namespace Web.Data
{
    public class ECommerceContext : DbContext
    {
        public ECommerceContext(DbContextOptions<ECommerceContext> options) : base(options) { }

        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<Kategori> Kategoriler { get; set; }
        public DbSet<Urun> Urunler { get; set; }
        public DbSet<Siparis> Siparisler { get; set; }
        public DbSet<Sepet> Sepetler { get; set; }
        public DbSet<SDurum> SDurumlari { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Kullanici>()
                .HasIndex(k => k.Email)
                .IsUnique();

            modelBuilder.Entity<Urun>()
                .HasOne(u => u.Kategori)
                .WithMany(k => k.Urunler)
                .HasForeignKey(u => u.KategoriId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Siparis>()
                .HasOne(s => s.Kullanici)
                .WithMany(k => k.Siparisler)
                .HasForeignKey(s => s.KullaniciId);

            modelBuilder.Entity<Siparis>()
                .HasOne(s => s.Urun)
                .WithMany(u => u.Siparisler)
                .HasForeignKey(s => s.UrunId);

            modelBuilder.Entity<Kategori>()
                .HasIndex(k => k.Ad)
                .IsUnique();

            modelBuilder.Entity<Urun>()
                .Property(u => u.Stok)
                .HasDefaultValue(0)
                .IsRequired();

            modelBuilder.Entity<Siparis>()
                .Property(s => s.Tarih)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Sepet>()
                .HasOne(s => s.Kullanici)
                .WithMany(k => k.Sepetler)
                .HasForeignKey(s => s.K_id);

            modelBuilder.Entity<Sepet>()
                .HasOne(s => s.Urun)
                .WithMany(u => u.Sepetler)
                .HasForeignKey(s => s.U_id);
        }
    }
}