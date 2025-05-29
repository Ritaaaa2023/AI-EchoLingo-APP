public class DiaryEntryTag
{
  public int DiaryEntryId { get; set; }
  public required DiaryEntry DiaryEntry { get; set; }

  public int TagId { get; set; }
  public required Tag Tag { get; set; }
}
