using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Environment2D
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid(); // ✅ Automatically generate a GUID

    [Required]
    public string Name { get; set; }

    [Required]
    public int MaxHeight { get; set; }

    [Required]
    public int MaxWidth { get; set; }

    [Required]
    [Column(TypeName = "uniqueidentifier")] // ✅ Ensures correct DB column type
    public Guid AppUserId { get; set; }
}