using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SatTrack.DAL;

[Table("sat_groups_cats", Schema = "sattracker")]
public partial class SatGroupsCat
{
    [Key]
    [Column("sat_group_cat_id")]
    public int SatGroupCatId { get; set; }

    [Column("description")]
    public string Description { get; set; } = null!;

    [InverseProperty("SatGroupCat")]
    public virtual ICollection<SatGroup> SatGroups { get; set; } = new List<SatGroup>();
}
