using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    [Table("sepet")]
    public class Sepet
    {
        [Key]
        [Column("sepetid")]
        public int Id { get; set; }

        [Column("kullanici_id")]
        public int K_id { get; set; }

        [ForeignKey("K_id")]
        public Kullanici Kullanici { get; set; } = null!;

        [Column("urun_id")]
        public int U_id { get; set; }

        [ForeignKey("U_id")]
        public Urun Urun { get; set; } = null!;

        [Column("miktar")]
        public int Miktar { get; set; }
    }
}