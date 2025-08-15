using Microsoft.AspNetCore.Identity;

namespace CareManagement.Auth.Core.Entities;

public class Role : IdentityRole<int>
{
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
}
