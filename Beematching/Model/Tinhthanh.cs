using System;
using System.Collections.Generic;

namespace Beematching.Model;

public partial class Tinhthanh
{
    public int Id { get; set; }

    public string? TenTinhThanh { get; set; }

    public virtual ICollection<Congviec> Congviecs { get; set; } = new List<Congviec>();

    public virtual ICollection<Doanhnghiep> Doanhnghieps { get; set; } = new List<Doanhnghiep>();
}
