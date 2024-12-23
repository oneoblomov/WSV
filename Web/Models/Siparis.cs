using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    [Table("siparisler")]
    public class Siparis
    {
        [Key]
        [Column("siparis_id")]
        public int Id { get; set; }

        [Column("kullanici_id")]
        public int KullaniciId { get; set; }

        [ForeignKey("KullaniciId")]
        public Kullanici Kullanici { get; set; } = null!;

        [Column("urun_id")]
        public int UrunId { get; set; }

        [ForeignKey("UrunId")]
        public Urun Urun { get; set; } = null!;

        [Column("miktar")]
        public int Miktar { get; set; }

        [Column("fiyat")]
        public decimal Fiyat { get; set; }

        [Column("tarih")]
        public DateTime Tarih { get; set; } = DateTime.UtcNow;

        [Column("siparis_durum_id")]
        public int SiparisDurumId { get; set; }

        [ForeignKey("SiparisDurumId")]
        public SDurum SiparisDurum { get; set; } = null!;
    }
}