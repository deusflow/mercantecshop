using System;
using System.Collections.Generic;

namespace WebShopMercantec.Models;

public partial class Setting
{
    public uint Id { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int PerPage { get; set; }

    public string SiteName { get; set; } = null!;

    public int? QrCode { get; set; }

    public string? QrText { get; set; }

    public int? DisplayAssetName { get; set; }

    public int? DisplayCheckoutDate { get; set; }

    public int? DisplayEol { get; set; }

    public int AutoIncrementAssets { get; set; }

    public string? AutoIncrementPrefix { get; set; }

    public bool? LoadRemote { get; set; }

    public string? Logo { get; set; }

    public string? HeaderColor { get; set; }

    public string? AlertEmail { get; set; }

    public bool? AlertsEnabled { get; set; }

    public string? DefaultEulaText { get; set; }

    public string? WebhookEndpoint { get; set; }

    public string? WebhookChannel { get; set; }

    public string? WebhookBotname { get; set; }

    public string? WebhookSelected { get; set; }

    public string? DefaultCurrency { get; set; }

    public string? CustomCss { get; set; }

    public sbyte Brand { get; set; }

    public string? LdapEnabled { get; set; }

    public string? LdapServer { get; set; }

    public string? LdapUname { get; set; }

    public string? LdapPword { get; set; }

    public string? LdapBasedn { get; set; }

    public string? LdapDefaultGroup { get; set; }

    public string? LdapFilter { get; set; }

    public string? LdapUsernameField { get; set; }

    public string? LdapLnameField { get; set; }

    public string? LdapFnameField { get; set; }

    public string? LdapAuthFilterQuery { get; set; }

    public int? LdapVersion { get; set; }

    public string? LdapActiveFlag { get; set; }

    public string? LdapDept { get; set; }

    public string? LdapEmpNum { get; set; }

    public string? LdapEmail { get; set; }

    public string? LdapPhoneField { get; set; }

    public string? LdapJobtitle { get; set; }

    public string? LdapManager { get; set; }

    public string? LdapCountry { get; set; }

    public string? LdapLocation { get; set; }

    public bool FullMultipleCompaniesSupport { get; set; }

    public bool LdapServerCertIgnore { get; set; }

    public string? Locale { get; set; }

    public sbyte LabelsPerPage { get; set; }

    public decimal LabelsWidth { get; set; }

    public decimal LabelsHeight { get; set; }

    public decimal LabelsPmarginLeft { get; set; }

    public decimal LabelsPmarginRight { get; set; }

    public decimal LabelsPmarginTop { get; set; }

    public decimal LabelsPmarginBottom { get; set; }

    public decimal LabelsDisplayBgutter { get; set; }

    public decimal LabelsDisplaySgutter { get; set; }

    public sbyte LabelsFontsize { get; set; }

    public decimal LabelsPagewidth { get; set; }

    public decimal LabelsPageheight { get; set; }

    public sbyte LabelsDisplayName { get; set; }

    public sbyte LabelsDisplaySerial { get; set; }

    public sbyte LabelsDisplayTag { get; set; }

    public bool? AltBarcodeEnabled { get; set; }

    public int? AlertInterval { get; set; }

    public int? AlertThreshold { get; set; }

    public string? NameDisplayFormat { get; set; }

    public string? EmailDomain { get; set; }

    public string? EmailFormat { get; set; }

    public string? UsernameFormat { get; set; }

    public bool IsAd { get; set; }

    public string? AdDomain { get; set; }

    public string LdapPort { get; set; } = null!;

    public bool LdapTls { get; set; }

    public int ZerofillCount { get; set; }

    public bool? LdapPwSync { get; set; }

    public sbyte? TwoFactorEnabled { get; set; }

    public bool RequireAcceptSignature { get; set; }

    public string DateDisplayFormat { get; set; } = null!;

    public string TimeDisplayFormat { get; set; } = null!;

    public long NextAutoTagBase { get; set; }

    public string? LoginNote { get; set; }

    public int? ThumbnailMaxH { get; set; }

    public bool PwdSecureUncommon { get; set; }

    public string? PwdSecureComplexity { get; set; }

    public int PwdSecureMin { get; set; }

    public int? AuditInterval { get; set; }

    public int? AuditWarningDays { get; set; }

    public bool ShowUrlInEmails { get; set; }

    public string? CustomForgotPassUrl { get; set; }

    public bool? ShowAlertsInMenu { get; set; }

    public bool LabelsDisplayCompanyName { get; set; }

    public bool ShowArchivedInList { get; set; }

    public string? DashboardMessage { get; set; }

    public string? SupportFooter { get; set; }

    public string? FooterText { get; set; }

    public string? ModellistDisplays { get; set; }

    public bool LoginRemoteUserEnabled { get; set; }

    public bool LoginCommonDisabled { get; set; }

    public string LoginRemoteUserCustomLogoutUrl { get; set; } = null!;

    public string? Skin { get; set; }

    public bool? ShowImagesInEmail { get; set; }

    public string? AdminCcEmail { get; set; }

    public bool LabelsDisplayModel { get; set; }

    public string? PrivacyPolicyLink { get; set; }

    public string? VersionFooter { get; set; }

    public bool UniqueSerial { get; set; }

    public bool LogoPrintAssets { get; set; }

    public string? DepreciationMethod { get; set; }

    public string? Favicon { get; set; }

    public string? DefaultAvatar { get; set; }

    public string? EmailLogo { get; set; }

    public string? LabelLogo { get; set; }

    public bool AllowUserSkin { get; set; }

    public bool ShowAssignedAssets { get; set; }

    public string LoginRemoteUserHeaderName { get; set; } = null!;

    public bool AdAppendDomain { get; set; }

    public bool SamlEnabled { get; set; }

    public string? SamlIdpMetadata { get; set; }

    public string? SamlAttrMappingUsername { get; set; }

    public bool SamlForcelogin { get; set; }

    public bool SamlSlo { get; set; }

    public string? SamlSpX509cert { get; set; }

    public string? SamlSpPrivatekey { get; set; }

    public string? SamlCustomSettings { get; set; }

    public string? SamlSpX509certNew { get; set; }

    public string? DigitSeparator { get; set; }

    public string? LdapClientTlsCert { get; set; }

    public string? LdapClientTlsKey { get; set; }

    public string? DashChartType { get; set; }

    public bool Label2Enable { get; set; }

    public string? Label2Template { get; set; }

    public string? Label2Title { get; set; }

    public bool Label2AssetLogo { get; set; }

    public string Label21dType { get; set; } = null!;

    public string Label22dType { get; set; } = null!;

    public string Label22dTarget { get; set; } = null!;

    public string Label2Fields { get; set; } = null!;

    public bool? GoogleLogin { get; set; }

    public string? GoogleClientId { get; set; }

    public string? GoogleClientSecret { get; set; }

    public bool? ProfileEdit { get; set; }

    public bool? RequireCheckinoutNotes { get; set; }

    public bool ShortcutsEnabled { get; set; }

    public int? DueCheckinDays { get; set; }
}
