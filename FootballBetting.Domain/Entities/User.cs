using System.ComponentModel.DataAnnotations;

namespace FootballBetting.Domain.Entities;

public class User
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;
    
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual ICollection<Bet> Bets { get; set; } = new List<Bet>();
    public virtual ICollection<UserProviderCredential> ProviderCredentials { get; set; } = new List<UserProviderCredential>();
}