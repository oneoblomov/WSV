using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    [Table("kategori")]
    public class Kategori
    {
        [Key]
        [Column("kategori_id")]
        public int Id { get; set; }

        [Column("ad")]
        public string Ad { get; set; } = string.Empty;

        [Column("aciklama")]
        public string Aciklama { get; set; } = string.Empty;

        public ICollection<Urun> Urunler { get; set; } = new List<Urun>();
    }
}