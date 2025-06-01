using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nhom9.Models
{
    public class DanhGia
    {
        [Key]
        public int MaDanhGia { get; set; }

        [Required]
        public int MaSP { get; set; }  // khóa ngoại đến sản phẩm

        [Required]
        [Range(1, 5, ErrorMessage = "Số sao phải từ 1 đến 5")]
        public int SoSao { get; set; }

        [Required]
        [StringLength(100)]
        public string HoTen { get; set; }

        [Required]
        public string NoiDung { get; set; }

        public DateTime NgayDanhGia { get; set; } = DateTime.Now;

        // Navigation property
        [ForeignKey("MaSP")]
        public virtual SanPham SanPham { get; set; }
    }
}
