public class DiaryEntry
{
  public int Id { get; set; }
  public string UserId { get; set; } = string.Empty;
  public required ApplicationUser User { get; set; }

  public required string Title { get; set; }
  public required string ChineseText { get; set; }
  public string? TranslatedText { get; set; }
  public string? NativeText { get; set; }
  public string? AudioUrl { get; set; }
  public float? Score { get; set; }
  public string? Mood { get; set; }

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime? UpdatedAt { get; set; }

  public ICollection<DiaryEntryTag>? DiaryEntryTags { get; set; }
}
