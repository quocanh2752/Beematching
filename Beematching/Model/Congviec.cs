using System;
using System.Collections.Generic;

namespace Beematching.Model;

public partial class Congviec
{
    public int Id { get; set; }

    public int IdDoanhNghiep { get; set; }

    public string? TenCongViec { get; set; }

    public int LoaiCongViec { get; set; }

    public string? MoTa { get; set; }

    public string? HinhAnh { get; set; }

    public string? DiaChi { get; set; }

    public int IdTinhThanh { get; set; }

    public decimal? MucLuong { get; set; }

    public string? YeuCauCongViec { get; set; }

    public DateTime? NgayTao { get; set; }

    public int? SoLuongUngTuyen { get; set; }

    public string? HinhThucCongViec { get; set; }

    public DateTime? NgayHetHan { get; set; }

    public virtual Doanhnghiep IdDoanhNghiepNavigation { get; set; } = null!;

    public virtual Tinhthanh IdTinhThanhNavigation { get; set; } = null!;

    public virtual Loaicongviec LoaiCongViecNavigation { get; set; } = null!;

    public virtual ICollection<Ungtuyen> Ungtuyens { get; set; } = new List<Ungtuyen>();
}
