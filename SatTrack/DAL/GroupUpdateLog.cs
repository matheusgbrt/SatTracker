using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SatTrack.DAL;

[Table("groupupdatelogs", Schema = "sattracker")]
public partial class GroupUpdateLog
{
    [Key]
    [Column("log_id")]
    public int LogId { get; set; }

    [Column("message")]
    public string? Message { get; set; }
    [Column("group_name")]
    public string? GroupName { get; set; }

    [Column("timestamp")]
    public DateTime Timestamp { get; set; }
    [Column("is_error")]
    public bool IsError { get; set; }

    [Column("active")]
    public bool Active { get; set; }
}
