using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APICodeMetrics.Models;

[Table("analysis_results")] // Имя таблицы в базе данных
public class AnalysisResult
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public DateTime AnalysisDate { get; set; } // Дата/время выполнения анализа

    // Поле для хранения текстового результата. Используем TEXT для длинных строк.
    [Required]
    [MaxLength(int.MaxValue)]
    public string Content { get; set; } = string.Empty;

    // Опционально: можно добавить поле для версии модели или другого контекста
    public string? AnalysisVersion { get; set; } = string.Empty;
}