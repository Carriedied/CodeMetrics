using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APICodeMetrics.Models;

[Table("repo_files")]
public class RepoFile
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty; // Для хранения диффа или содержимого файла

    // Связь с коммитом
    [ForeignKey("Commit")]
    public int CommitId { get; set; }
    public RepoCommit Commit { get; set; } = null!;
}