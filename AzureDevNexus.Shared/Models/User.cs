namespace AzureDevNexus.Shared.Models;

public class User
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string AzureAdId { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime LastLoginDate { get; set; }
    public bool IsActive { get; set; }
    public List<string> Permissions { get; set; } = new();
}

public class UserProfile
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string TimeZone { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
}
