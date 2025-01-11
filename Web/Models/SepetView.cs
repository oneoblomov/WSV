using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace Web.Models
{
    [Table("sepet_view")]
    public class SepetView
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("kullaniciid")]
        public int KullaniciId { get; set; }

        [Column("urunid")]
        public int UrunId { get; set; }

        [Column("urunad")]
        public string UrunAd { get; set; } = string.Empty;

        [Column("miktar")]
        public int Miktar { get; set; }

        [Column("fiyat")]
        public decimal Fiyat { get; set; }
    }
}