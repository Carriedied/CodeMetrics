using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APICodeMetrics.Models;

[Table("repo_branches")]
public class RepoBranch
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    public bool IsProtected { get; set; }

    [MaxLength(255)]
    public string TargetBranch { get; set; } = string.Empty;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // Связь с репозиторием
    [ForeignKey("Repository")]
    public int RepositoryId { get; set; }
    public Repository Repository { get; set; } = null!;

    // Связь с коммитами
    public ICollection<RepoCommit> Commits { get; set; } = new List<RepoCommit>();
}