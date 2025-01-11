using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    [Table("kullanici_view")]
    public class KullaniciView
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("ad")]
        public string Ad { get; set; } = string.Empty;

        [Column("soyad")]
        public string Soyad { get; set; } = string.Empty;

        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Column("telefon")]
        public string Telefon { get; set; } = string.Empty;

        [Column("kayittarihi")]
        public DateTime KayitTarihi { get; set; }
    }
}