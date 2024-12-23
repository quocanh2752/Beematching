using System;
using System.Collections.Generic;

namespace Beematching.Model;

public partial class Thongbao
{
    public int Id { get; set; }

    public int IdNguoiDung { get; set; }

    public string? TieuDe { get; set; }

    public string? NoiDung { get; set; }

    public string? TrangThai { get; set; }

    public DateTime? NgayThongBao { get; set; }

    public virtual Nguoidung IdNguoiDungNavigation { get; set; } = null!;
}
