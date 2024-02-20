using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HelloDoc.DataModels;

[Table("users")]
public partial class User
{
    [Key]
    [Column("userid")]
    public int Userid { get; set; }

    [Column("aspnetuserid")]
    [StringLength(128)]
    public string Aspnetuserid { get; set; } = null!;

    [Column("firstname")]
    [StringLength(100)]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Enter Valid Firstname")]
    [Required(ErrorMessage = "Firstname is required")]
    public string Firstname { get; set; } = null!;

    [Column("lastname")]
    [StringLength(100)]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Enter a Valid Lastname")]
    [Required(ErrorMessage = "Last Name is required")]

    public string? Lastname { get; set; }

    [Column("email")]
    [StringLength(50)]
    public string Email { get; set; } = null!;

    [Column("mobile")]
    [StringLength(20)]
    [RegularExpression(@"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$", ErrorMessage = "Please enter valid phone number")]
    [Required(ErrorMessage = "Plese enter your Phone Number")]

    public string? Mobile { get; set; }

    [Column("ismobile", TypeName = "bit(1)")]
    public BitArray? Ismobile { get; set; }

    [Column("street")]
    [Required(ErrorMessage = "Street is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Enter valid Street")]
    [RegularExpression(@"^(?=.*\S)[a-zA-Z0-9\s.,'-]+$", ErrorMessage = "Enter a valid street address")]

    public string? Street { get; set; }

    [Column("city")]
    [Required(ErrorMessage = "City is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Enter valid City")]
    [RegularExpression(@"^(?=.*\S)[a-zA-Z\s.'-]+$", ErrorMessage = "Enter a valid city name")]

    public string? City { get; set; }

    [Column("state")]
    [Required(ErrorMessage = "State is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Enter valid State")]
    [RegularExpression(@"^(?=.*\S)[a-zA-Z\s.'-]+$", ErrorMessage = "Enter a valid State name")]
    //[RegularExpression(@"^[A-Z]{2}$", ErrorMessage = "Enter a valid state abbreviation (e.g., NY, CA)")]
    public string? State { get; set; }

    [Column("regionid")]
    public int? Regionid { get; set; }

    [Column("zip")]
    [Required(ErrorMessage = "Zip Code is required")]
    [StringLength(10, ErrorMessage = "Enter valid Zip Code")]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "Enter a valid 6-digit zip code")]
    public string? Zip { get; set; }

    [Column("strmonth")]
    [StringLength(20)]
    public string? Strmonth { get; set; }

    [Column("intyear")]
    public int? Intyear { get; set; }

    [Column("intdate")]
    public int? Intdate { get; set; }

    [Column("createdby")]
    [StringLength(128)]
    public string Createdby { get; set; } = null!;

    [Column("createddate", TypeName = "timestamp without time zone")]
    public DateTime Createddate { get; set; }

    [Column("modifiedby")]
    [StringLength(128)]
    public string? Modifiedby { get; set; }

    [Column("modifieddate", TypeName = "timestamp without time zone")]
    public DateTime? Modifieddate { get; set; }

    [Column("status")]
    public short? Status { get; set; }

    [Column("isdeleted", TypeName = "bit(1)")]
    public BitArray? Isdeleted { get; set; }

    [Column("ip")]
    [StringLength(20)]
    public string? Ip { get; set; }

    [Column("isrequestwithemail", TypeName = "bit(1)")]
    public BitArray? Isrequestwithemail { get; set; }

    [ForeignKey("Aspnetuserid")]
    [InverseProperty("Users")]
    public virtual Aspnetuser Aspnetuser { get; set; } = null!;

    [ForeignKey("Regionid")]
    [InverseProperty("Users")]
    public virtual Region? Region { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
