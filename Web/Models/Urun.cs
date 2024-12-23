using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    [Table("urun")]
    public class Urun
    {
        [Key]
        [Column("urun_id")]
        public int Id { get; set; }

        [Column("ad")]
        public string Ad { get; set; }

        [Column("aciklama")]
        public string Aciklama { get; set; }

        [Column("gorselyolu")]
        public string Gorsel { get; set; }

        [Column("fiyat")]
        public decimal Fiyat { get; set; }

        [Column("stok")]
        public int Stok { get; set; }

        [Column("kategori_id")]
        public int KategoriId { get; set; }

        [ForeignKey("KategoriId")]
        public Kategori Kategori { get; set; } = null!;

        [Column("gor_sayi")]
        public int GorSayi { get; set; }

        public ICollection<Siparis> Siparisler { get; set; } = new List<Siparis>();
        public ICollection<Sepet> Sepetler { get; set; } = new List<Sepet>();
    }
}