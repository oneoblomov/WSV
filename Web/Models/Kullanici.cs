using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    [Table("kullanicilar")] // Tablo adını burada güncelleyin
    public class Kullanici
    {
        [Key]
        [Column("kullanici_id")]
        public int Id { get; set; }

        [Column("ad")]
        public string Ad { get; set; } = string.Empty;

        [Column("soyad")]
        public string Soyad { get; set; } = string.Empty;

        [Column("eposta")]
        public string Email { get; set; } = string.Empty;

        [Column("sifre")]
        public string Sifre { get; set; } = string.Empty;

        [Column("telefon")]
        public string Telefon { get; set; } = string.Empty;

        [Column("kayittarihi")]
        public DateTime KayitTarihi { get; set; }

        public ICollection<Siparis> Siparisler { get; set; } = new List<Siparis>();
        public ICollection<Sepet> Sepetler { get; set; } = new List<Sepet>();
    }
}