using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SatTrack.DAL;

[Table("sat_groups", Schema = "sattracker")]
public partial class SatGroup
{
    [Key]
    [Column("sat_group_id")]
    public int SatGroupId { get; set; }

    [Column("sat_group_cat_id")]
    public int SatGroupCatId { get; set; }

    [Column("sat_group_name")]
    public string SatGroupName { get; set; } = null!;

    [Column("sat_group_query")]
    public string SatGroupQuery { get; set; } = null!;

    [Column("last_update_date", TypeName = "timestamp without time zone")]
    public DateTime? LastUpdateDate { get; set; }

    [ForeignKey("SatGroupCatId")]
    [InverseProperty("SatGroups")]
    public virtual SatGroupsCat SatGroupCat { get; set; } = null!;

    [ForeignKey("SatGroupId")]
    [InverseProperty("SatGroups")]
    public virtual ICollection<Sat> Sats { get; set; } = new List<Sat>();
}
