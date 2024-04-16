using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelloDoc;

[PrimaryKey("Userid", "Roleid")]
[Table("aspnetuserroles")]
public partial class Aspnetuserrole
{
    [Key]
    [Column("userid")]
    [StringLength(128)]
    public string Userid { get; set; } = null!;

    [Key]
    [Column(TypeName = "character varying")]
    public string Roleid { get; set; } = null!;

    [ForeignKey("Userid")]
    [InverseProperty("Aspnetuserroles")]
    public virtual Aspnetuser User { get; set; } = null!;
}
