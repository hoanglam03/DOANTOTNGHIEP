using System;
using System.ComponentModel.DataAnnotations;

namespace Nhom9.Models // hoặc Nhom9.Areas.Admin.Models nếu bạn để ở admin
{
    public class TinTuc
    {
        [Key]
        public int MaTin { get; set; }

        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        [StringLength(255, ErrorMessage = "Tiêu đề không vượt quá 255 ký tự")]

        [Display(Name = "Tiêu đề")]
        public string TieuDe { get; set; }

        [Required(ErrorMessage = "Nội dung không được để trống")]
        [Display(Name = "Nội dung")]
        public string NoiDung { get; set; }

        [StringLength(500)]
        [Display(Name = "Mô tả ngắn ")]
        public string MoTaNgan { get; set; }

        public string HinhAnh { get; set; } // đường dẫn ảnh
        [Display(Name = "Ngày đăng ")]
        public DateTime NgayDang { get; set; } = DateTime.Now;
        [Display(Name = "Trạng thái")]
        public bool IsActive { get; set; } = true;
    }
}