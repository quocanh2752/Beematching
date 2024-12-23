using System;
using System.Collections.Generic;

namespace Beematching.Model;

public partial class Nguoidung
{
    public int Id { get; set; }

    public string? HoTen { get; set; }

    public string? MatKhau { get; set; }

    public string? Email { get; set; }

    public string? VaiTro { get; set; }

    public string? TrangThai { get; set; }

    public DateTime? NgayTao { get; set; }

    public virtual ICollection<Doanhnghiep> Doanhnghieps { get; set; } = new List<Doanhnghiep>();

    public virtual ICollection<Nguoixinviec> Nguoixinviecs { get; set; } = new List<Nguoixinviec>();

    public virtual ICollection<Thongbao> Thongbaos { get; set; } = new List<Thongbao>();
}
