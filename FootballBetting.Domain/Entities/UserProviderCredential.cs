using System.ComponentModel.DataAnnotations;
using FootballBetting.Domain.Enums;

namespace FootballBetting.Domain.Entities;

/// <summary>
/// Stores encrypted credentials for each betting provider per user
/// </summary>
public class UserProviderCredential
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    public virtual User User { get; set; } = null!;
    
    public BettingProvider Provider { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(500)]
    public string EncryptedPassword { get; set; } = string.Empty;
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
    public DateTime? LastUsedAt { get; set; }
    
    [MaxLength(1000)]
    public string? Notes { get; set; }
}