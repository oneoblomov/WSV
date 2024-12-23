using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    [Table("siparis_durumlari")]
    public class SDurum
    {
        [Key]
        [Column("durum_id")]
        public int Id { get; set; }

        [Column("durum_adi")]
        public string D_Adi { get; set; } = string.Empty;
    }
}