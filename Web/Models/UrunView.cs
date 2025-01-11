using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    [Table("urunler_view")]
    public class UrunView
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("ad")]
        public string Ad { get; set; } = string.Empty;

        [Column("aciklama")]
        public string Aciklama { get; set; } = string.Empty;

        [Column("gorsel")]
        public string Gorsel { get; set; } = string.Empty;

        [Column("fiyat")]
        public decimal Fiyat { get; set; }

        [Column("stok")]
        public int Stok { get; set; }

        [Column("kategoriid")]
        public int KategoriId { get; set; }

        [Column("kategoriad")]
        public string KategoriAd { get; set; } = string.Empty;
    }
}