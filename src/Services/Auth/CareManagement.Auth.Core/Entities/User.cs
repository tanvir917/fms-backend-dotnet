using Microsoft.AspNetCore.Identity;

namespace CareManagement.Auth.Core.Entities;

public class User : IdentityUser<int>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;

    // Domain methods
    public string GetFullName() => $"{FirstName} {LastName}".Trim();

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    public void UpdateProfile(string firstName, string lastName, string email)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        NormalizedEmail = email.ToUpperInvariant();
        UpdatedAt = DateTime.UtcNow;
    }
}
