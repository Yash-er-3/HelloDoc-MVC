using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HelloDoc.DataModels;
using Microsoft.EntityFrameworkCore;

namespace HelloDoc;

[Table("encounter")]
public partial class Encounter
{
    [Key]
    public int Id { get; set; }

    public int RequestId { get; set; }

    [Column("isFinalized", TypeName = "bit(1)")]
    public BitArray? IsFinalized { get; set; }

    [StringLength(200)]
    public string? HistoryIllness { get; set; }

    [StringLength(200)]
    public string? MedicalHistory { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? Date { get; set; }

    [StringLength(200)]
    public string? Medications { get; set; }

    [StringLength(200)]
    public string? Allergies { get; set; }

    public decimal? Temp { get; set; }

    [Column("HR")]
    public decimal? Hr { get; set; }

    [Column("RR")]
    public decimal? Rr { get; set; }

    [Column("BP(S)")]
    public int? BpS { get; set; }

    [Column("BP(D)")]
    public int? BpD { get; set; }

    public decimal? O2 { get; set; }

    [StringLength(200)]
    public string? Pain { get; set; }

    [Column("HEENT")]
    [StringLength(200)]
    public string? Heent { get; set; }

    [Column("CV")]
    [StringLength(200)]
    public string? Cv { get; set; }

    [StringLength(200)]
    public string? Chest { get; set; }

    [Column("ABD")]
    [StringLength(200)]
    public string? Abd { get; set; }

    [StringLength(200)]
    public string? Extr { get; set; }

    [StringLength(200)]
    public string? Skin { get; set; }

    [StringLength(200)]
    public string? Neuro { get; set; }

    [StringLength(200)]
    public string? Other { get; set; }

    [StringLength(200)]
    public string? Diagnosis { get; set; }

    [StringLength(200)]
    public string? TreatmentPlan { get; set; }

    [StringLength(200)]
    public string? MedicationDispensed { get; set; }

    [StringLength(200)]
    public string? Procedures { get; set; }

    [StringLength(200)]
    public string? FollowUp { get; set; }

    [Column("createddate", TypeName = "timestamp without time zone")]
    public DateTime? Createddate { get; set; }

    [Column("createdby")]
    [StringLength(128)]
    public string? Createdby { get; set; }

    [Column("modifieddate", TypeName = "timestamp without time zone")]
    public DateTime? Modifieddate { get; set; }

    [Column("modifiedby")]
    [StringLength(128)]
    public string? Modifiedby { get; set; }

    [ForeignKey("RequestId")]
    [InverseProperty("Encounters")]
    public virtual Request Request { get; set; } = null!;
}
