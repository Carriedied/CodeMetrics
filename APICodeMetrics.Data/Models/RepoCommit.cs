using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APICodeMetrics.Models;

[Table("repo_commits")]
public class RepoCommit
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(int.MaxValue)]
    public string Sha1 { get; set; } = string.Empty;

    [Required]
    [MaxLength(int.MaxValue)]
    public string Message { get; set; } = string.Empty;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // Связь с автором
    [ForeignKey("Author")]
    public int AuthorId { get; set; }
    public GitUser Author { get; set; } = null!;

    // Связь с коммиттером
    [ForeignKey("Committer")]
    public int CommitterId { get; set; }
    public GitUser Committer { get; set; } = null!;

    // Связь с родительским коммитом (если нужно)
    // public string ParentSha1 { get; set; } = string.Empty;

    // Связь с веткой
    [ForeignKey("Branch")]
    public int BranchId { get; set; }
    public RepoBranch Branch { get; set; } = null!;

    // Связь с файлами (если нужно хранить диффы в отдельной таблице)
    public ICollection<RepoFile> Files { get; set; } = new List<RepoFile>();
}