using System;
using System.Collections.Generic;

namespace WebShopMercantec.Models;

public partial class User
{
    public uint Id { get; set; }

    public string? Email { get; set; }

    public string Password { get; set; } = null!;

    public string? Permissions { get; set; }

    public bool Activated { get; set; }

    public int? CreatedBy { get; set; }

    public string? ActivationCode { get; set; }

    public DateTime? ActivatedAt { get; set; }

    public DateTime? LastLogin { get; set; }

    public string? PersistCode { get; set; }

    public string? ResetPasswordCode { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? Website { get; set; }

    public string? Country { get; set; }

    public string? Gravatar { get; set; }

    public int? LocationId { get; set; }

    public string? Phone { get; set; }

    public string? Jobtitle { get; set; }

    public int? ManagerId { get; set; }

    public string? EmployeeNum { get; set; }

    public string? Avatar { get; set; }

    public string? Username { get; set; }

    public string? Notes { get; set; }

    public uint? CompanyId { get; set; }

    public string? RememberToken { get; set; }

    public bool LdapImport { get; set; }

    public string? Locale { get; set; }

    public bool? ShowInList { get; set; }

    public string? TwoFactorSecret { get; set; }

    public bool TwoFactorEnrolled { get; set; }

    public bool TwoFactorOptin { get; set; }

    public int? DepartmentId { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? Zip { get; set; }

    public string? Skin { get; set; }

    public bool? Remote { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? ScimExternalid { get; set; }

    public bool? AutoassignLicenses { get; set; }

    public bool? Vip { get; set; }

    public bool EnableSounds { get; set; }

    public bool EnableConfetti { get; set; }
}
