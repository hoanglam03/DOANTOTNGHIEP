namespace Nhom9.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web.Script.Serialization;

    [Table("TaiKhoanNguoiDung")]
    public partial class TaiKhoanNguoiDung
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TaiKhoanNguoiDung()
        {
            HoaDons = new HashSet<HoaDon>();
        }

        [Key]
        public int MaTK { get; set; }

        [Required]
        [StringLength(100)]
        public string TenDangNhap { get; set; }

        [Required]
        [StringLength(50)]
        public string MatKhau { get; set; }

        [Required]
        [StringLength(100)]
        public string HoTen { get; set; }

        [Required]
        [StringLength(11)]
        public string SoDienThoai { get; set; }

        [Required]
        [StringLength(100)]
        public string DiaChi { get; set; }

        public DateTime NgaySinh { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        public bool GioiTinh { get; set; }

        public bool TrangThai { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [ScriptIgnore]
        public virtual ICollection<HoaDon> HoaDons { get; set; }
    }
}
