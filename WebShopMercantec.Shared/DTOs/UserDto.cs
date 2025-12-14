namespace WebShopMercantec.Shared.DTOs;

/// <summary>
/// DTO for User (employees/customers who use the shop)
/// Used for displaying user information, managing accounts, and tracking credits
/// Contains profile information, role permissions, and credit balance
/// </summary>
public class UserDto
{
    // Main fields
    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    // User's name
    public string FirstName { get; set; } = string.Empty;
    
    public string LastName { get; set; } = string.Empty;
    
    // Full name (computed)
    public string FullName => $"{FirstName} {LastName}".Trim();

    // Avatar/profile photo
    public string? Avatar { get; set; }
    
    // Initials for display (JD for John Doe)
    public string Initials => $"{FirstName.FirstOrDefault()}{LastName.FirstOrDefault()}".ToUpper();

    // Role/permissions
    public string Role { get; set; } = "User"; // User, Admin, Manager
    
    public string? Permissions { get; set; }

    // CREDITS - main currency in the system
    public decimal AvailableCredits { get; set; }
    
    public decimal TotalCreditsSpent { get; set; }

    // User statistics
    public int TotalPurchases { get; set; }

    // Contact information
    public string? Phone { get; set; }
    
    public string? Jobtitle { get; set; } // Note: lowercase 'title' to match model

    // Location and organization
    public int? LocationId { get; set; }
    
    public string? LocationName { get; set; }
    
    public int? DepartmentId { get; set; }
    
    public string? DepartmentName { get; set; }

    public int? CompanyId { get; set; }
    
    public string? CompanyName { get; set; }

    // Address
    public string? Address { get; set; }
    
    public string? City { get; set; }
    
    public string? State { get; set; }
    
    public string? Country { get; set; }
    
    public string? Zip { get; set; }

    // Manager
    public int? ManagerId { get; set; }
    
    public string? ManagerName { get; set; }

    // Employee number
    public string? EmployeeNum { get; set; } // Match model field name

    // Work dates
    public DateTime? StartDate { get; set; }
    
    public DateTime? EndDate { get; set; }

    // Account status
    public bool IsActive { get; set; } = true;
    
    public bool Activated { get; set; } // Match model field name
    
    public bool ShowInList { get; set; } = true;

    // VIP status (for special users)
    public bool Vip { get; set; } // Match model field name
    
    // Remote work
    public bool Remote { get; set; } // Match model field name

    // Timestamps
    public DateTime? CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    public DateTime? LastLogin { get; set; } // Match model field name

    // Additional information
    public string? Notes { get; set; }
    
    public string? Website { get; set; }
}

