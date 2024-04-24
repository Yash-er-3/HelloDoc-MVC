using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataModels.DataModels;

[Table("Student")]
public partial class Student
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string? FirstName { get; set; }

    [StringLength(100)]
    public string? LastName { get; set; }

    public int CourseId { get; set; }

    [Column(TypeName = "character varying")]
    public string? Age { get; set; }

    [Column(TypeName = "character varying")]
    public string? Email { get; set; }

  

    [Column(TypeName = "character varying")]
    public string? Course { get; set; }

    [Column(TypeName = "character varying")]
    public string? Grade { get; set; }

    [Column(TypeName = "character varying")]
    public string? Gender { get; set; }

    [ForeignKey("CourseId")]
    [InverseProperty("Students")]
    public virtual Course CourseNavigation { get; set; } = null!;
}
