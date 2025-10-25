using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APICodeMetrics.Models;

[Table("repositories")]
public class Repository
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(255)]
    public string Slug { get; set; } = string.Empty; // Можно использовать Name, если Slug не доступен

    [MaxLength(255)]
    public string? Description { get; set; }

    public bool IsFork { get; set; }

    [MaxLength(255)]
    public string OwnerName { get; set; } = string.Empty;

    // Ссылки на клонирование
    public string CloneLinkHttps { get; set; } = string.Empty;
    public string CloneLinkSsh { get; set; } = string.Empty;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // Связь с проектом
    [ForeignKey("Project")]
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    // Связь с ветками
    public ICollection<RepoBranch> Branches { get; set; } = new List<RepoBranch>();
}