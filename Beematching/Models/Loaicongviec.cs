using System;
using System.Collections.Generic;

namespace Beematching.Model;

public partial class Loaicongviec
{
    public int Id { get; set; }

    public string? TenLoaiCongViec { get; set; }

    public virtual ICollection<Congviec> Congviecs { get; set; } = new List<Congviec>();
}
