public class Tag
{
  public int Id { get; set; }
  public string? Name { get; set; }

  public ICollection<DiaryEntryTag>? DiaryEntryTags { get; set; }
}
