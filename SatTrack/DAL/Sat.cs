using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SatTrack.DAL;

[Table("sats", Schema = "sattracker")]
[Index("ObjectId", Name = "unique_object_id", IsUnique = true)]
public partial class Sat
{
    [Key]
    [Column("sat_id")]
    public int SatId { get; set; }

    [Column("object_name", TypeName = "character varying")]
    public string? ObjectName { get; set; }

    [Column("object_id", TypeName = "character varying")]
    public string? ObjectId { get; set; }

    [Column("mean_motion")]
    public decimal? MeanMotion { get; set; }

    [Column("eccentricity")]
    public decimal? Eccentricity { get; set; }

    [Column("inclination")]
    public decimal? Inclination { get; set; }

    [Column("ra_of_asc_node")]
    public decimal? RaOfAscNode { get; set; }

    [Column("arg_of_pericenter")]
    public decimal? ArgOfPericenter { get; set; }

    [Column("mean_anomaly")]
    public decimal? MeanAnomaly { get; set; }

    [Column("ephemeris_type")]
    public int? EphemerisType { get; set; }

    [Column("classification_type", TypeName = "character varying")]
    public string? ClassificationType { get; set; }

    [Column("norad_cat_id")]
    public int? NoradCatId { get; set; }

    [Column("element_set_no")]
    public int? ElementSetNo { get; set; }

    [Column("rev_at_epoch")]
    public int? RevAtEpoch { get; set; }

    [Column("bstar")]
    public decimal? Bstar { get; set; }

    [Column("mean_motion_dot")]
    public decimal? MeanMotionDot { get; set; }

    [Column("mean_motion_ddot")]
    public decimal? MeanMotionDdot { get; set; }

    [Column("epoch")]
    public DateTime? Epoch { get; set; }

    [ForeignKey("SatId")]
    [InverseProperty("Sats")]
    public virtual ICollection<SatGroup> SatGroups { get; set; } = new List<SatGroup>();
}
