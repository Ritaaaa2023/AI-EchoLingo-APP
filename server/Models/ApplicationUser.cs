using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
  public required string DisplayName { get; set; }
  public string? AvatarUrl { get; set; }
  public string SubscriptionStatus { get; set; } = "Free"; // Free / Trial / Subscribed
  public DateTime? TrialStartDate { get; set; }
  public DateTime? SubscriptionEndDate { get; set; }

  public string? StripeCustomerId { get; set; }
  public string? StripeSubscriptionId { get; set; }

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime? UpdatedAt { get; set; }
}
