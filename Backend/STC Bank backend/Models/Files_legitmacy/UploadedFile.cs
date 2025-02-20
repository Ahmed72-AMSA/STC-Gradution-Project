using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class UploadedFile
{
    [Key]
    public int Id { get; set; }

    public string? FileName { get; set; }

    public string? FileHash { get; set; }

    public bool? IsMalicious { get; set; }

    public string? Message { get; set; }

    public DateTime? UploadedAt { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "varbinary(max)")]
    public byte[]? FileData { get; set; }
}
