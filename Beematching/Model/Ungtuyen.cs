using System;
using System.Collections.Generic;

namespace Beematching.Model;

public partial class Ungtuyen
{
    public int Id { get; set; }

    public int IdNguoiXinViec { get; set; }

    public int IdCongViec { get; set; }

    public DateTime? NgayUngTuyen { get; set; }

    public string? YeuCauNguyenVong { get; set; }

    public string? TrangThai { get; set; }

    public virtual Congviec IdCongViecNavigation { get; set; } = null!;

    public virtual Nguoixinviec IdNguoiXinViecNavigation { get; set; } = null!;
}
