using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace WebShopMercantec.Models;

public partial class SnipeItContext : DbContext
{
    public SnipeItContext()
    {
    }

    public SnipeItContext(DbContextOptions<SnipeItContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AccessoriesCheckout> AccessoriesCheckouts { get; set; }

    public virtual DbSet<Accessory> Accessories { get; set; }

    public virtual DbSet<ActionLog> ActionLogs { get; set; }

    public virtual DbSet<Asset> Assets { get; set; }

    public virtual DbSet<AssetLog> AssetLogs { get; set; }

    public virtual DbSet<AssetMaintenance> AssetMaintenances { get; set; }

    public virtual DbSet<AssetUpload> AssetUploads { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<CheckoutAcceptance> CheckoutAcceptances { get; set; }

    public virtual DbSet<CheckoutRequest> CheckoutRequests { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Component> Components { get; set; }

    public virtual DbSet<ComponentsAsset> ComponentsAssets { get; set; }

    public virtual DbSet<Consumable> Consumables { get; set; }

    public virtual DbSet<ConsumablesUser> ConsumablesUsers { get; set; }

    public virtual DbSet<CustomField> CustomFields { get; set; }

    public virtual DbSet<CustomFieldCustomFieldset> CustomFieldCustomFieldsets { get; set; }

    public virtual DbSet<CustomFieldset> CustomFieldsets { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Depreciation> Depreciations { get; set; }

    public virtual DbSet<Import> Imports { get; set; }

    public virtual DbSet<Kit> Kits { get; set; }

    public virtual DbSet<KitsAccessory> KitsAccessories { get; set; }

    public virtual DbSet<KitsConsumable> KitsConsumables { get; set; }

    public virtual DbSet<KitsLicense> KitsLicenses { get; set; }

    public virtual DbSet<KitsModel> KitsModels { get; set; }

    public virtual DbSet<License> Licenses { get; set; }

    public virtual DbSet<LicenseSeat> LicenseSeats { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<LoginAttempt> LoginAttempts { get; set; }

    public virtual DbSet<Manufacturer> Manufacturers { get; set; }

    public virtual DbSet<Migration> Migrations { get; set; }

    public virtual DbSet<Model> Models { get; set; }

    public virtual DbSet<ModelsCustomField> ModelsCustomFields { get; set; }

    public virtual DbSet<OauthAccessToken> OauthAccessTokens { get; set; }

    public virtual DbSet<OauthAuthCode> OauthAuthCodes { get; set; }

    public virtual DbSet<OauthClient> OauthClients { get; set; }

    public virtual DbSet<OauthPersonalAccessClient> OauthPersonalAccessClients { get; set; }

    public virtual DbSet<OauthRefreshToken> OauthRefreshTokens { get; set; }

    public virtual DbSet<PasswordReset> PasswordResets { get; set; }

    public virtual DbSet<PermissionGroup> PermissionGroups { get; set; }

    public virtual DbSet<ReportTemplate> ReportTemplates { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<RequestedAsset> RequestedAssets { get; set; }

    public virtual DbSet<SamlNonce> SamlNonces { get; set; }

    public virtual DbSet<Setting> Settings { get; set; }

    public virtual DbSet<StatusLabel> StatusLabels { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<Throttle> Throttles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UsersGroup> UsersGroups { get; set; }

    /*
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=127.0.0.1;port=3307;database=snipeit;user=snipeit;password=YOURPASS;", Microsoft.EntityFrameworkCore.ServerVersion.Parse("11.5.2-mariadb"));
    */
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_uca1400_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<AccessoriesCheckout>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("accessories_checkout")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.AccessoryId)
                .HasColumnType("int(11)")
                .HasColumnName("accessory_id");
            entity.Property(e => e.AssignedTo)
                .HasColumnType("int(11)")
                .HasColumnName("assigned_to");
            entity.Property(e => e.AssignedType)
                .HasMaxLength(191)
                .HasColumnName("assigned_type");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasColumnType("int(11)")
                .HasColumnName("created_by");
            entity.Property(e => e.Note)
                .HasMaxLength(191)
                .HasColumnName("note");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Accessory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("accessories")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.CompanyId, "accessories_company_id_index");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.CategoryId)
                .HasColumnType("int(11)")
                .HasColumnName("category_id");
            entity.Property(e => e.CompanyId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasColumnType("int(11)")
                .HasColumnName("created_by");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Image)
                .HasMaxLength(191)
                .HasColumnName("image");
            entity.Property(e => e.LocationId)
                .HasColumnType("int(11)")
                .HasColumnName("location_id");
            entity.Property(e => e.ManufacturerId)
                .HasColumnType("int(11)")
                .HasColumnName("manufacturer_id");
            entity.Property(e => e.MinAmt)
                .HasColumnType("int(11)")
                .HasColumnName("min_amt");
            entity.Property(e => e.ModelNumber)
                .HasMaxLength(191)
                .HasColumnName("model_number");
            entity.Property(e => e.Name)
                .HasMaxLength(191)
                .HasColumnName("name");
            entity.Property(e => e.Notes)
                .HasColumnType("text")
                .HasColumnName("notes");
            entity.Property(e => e.OrderNumber)
                .HasMaxLength(191)
                .HasColumnName("order_number");
            entity.Property(e => e.PurchaseCost)
                .HasPrecision(20, 2)
                .HasColumnName("purchase_cost");
            entity.Property(e => e.PurchaseDate).HasColumnName("purchase_date");
            entity.Property(e => e.Qty)
                .HasColumnType("int(11)")
                .HasColumnName("qty");
            entity.Property(e => e.Requestable).HasColumnName("requestable");
            entity.Property(e => e.SupplierId)
                .HasColumnType("int(11)")
                .HasColumnName("supplier_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<ActionLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("action_logs")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.ActionType, "action_logs_action_type_index");

            entity.HasIndex(e => e.CompanyId, "action_logs_company_id_index");

            entity.HasIndex(e => e.CreatedAt, "action_logs_created_at_index");

            entity.HasIndex(e => new { e.ItemType, e.ItemId, e.ActionType }, "action_logs_item_type_item_id_action_type_index");

            entity.HasIndex(e => e.RemoteIp, "action_logs_remote_ip_index");

            entity.HasIndex(e => new { e.TargetType, e.TargetId, e.ActionType }, "action_logs_target_type_target_id_action_type_index");

            entity.HasIndex(e => new { e.TargetType, e.TargetId }, "action_logs_target_type_target_id_index");

            entity.HasIndex(e => e.ThreadId, "action_logs_thread_id_index");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.AcceptSignature)
                .HasMaxLength(100)
                .HasColumnName("accept_signature");
            entity.Property(e => e.AcceptedId)
                .HasColumnType("int(11)")
                .HasColumnName("accepted_id");
            entity.Property(e => e.ActionDate)
                .HasColumnType("datetime")
                .HasColumnName("action_date");
            entity.Property(e => e.ActionSource)
                .HasMaxLength(191)
                .HasColumnName("action_source");
            entity.Property(e => e.ActionType)
                .HasMaxLength(191)
                .HasColumnName("action_type");
            entity.Property(e => e.CompanyId)
                .HasColumnType("int(11)")
                .HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasColumnType("int(11)")
                .HasColumnName("created_by");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp")
                .HasColumnName("deleted_at");
            entity.Property(e => e.ExpectedCheckin).HasColumnName("expected_checkin");
            entity.Property(e => e.Filename)
                .HasColumnType("text")
                .HasColumnName("filename");
            entity.Property(e => e.ItemId)
                .HasColumnType("int(11)")
                .HasColumnName("item_id");
            entity.Property(e => e.ItemType)
                .HasMaxLength(191)
                .HasColumnName("item_type");
            entity.Property(e => e.LocationId)
                .HasColumnType("int(11)")
                .HasColumnName("location_id");
            entity.Property(e => e.LogMeta)
                .HasColumnType("text")
                .HasColumnName("log_meta");
            entity.Property(e => e.Note)
                .HasColumnType("text")
                .HasColumnName("note");
            entity.Property(e => e.RemoteIp)
                .HasMaxLength(45)
                .HasColumnName("remote_ip");
            entity.Property(e => e.StoredEula)
                .HasColumnType("text")
                .HasColumnName("stored_eula");
            entity.Property(e => e.TargetId)
                .HasColumnType("int(11)")
                .HasColumnName("target_id");
            entity.Property(e => e.TargetType)
                .HasMaxLength(191)
                .HasColumnName("target_type");
            entity.Property(e => e.ThreadId)
                .HasColumnType("int(11)")
                .HasColumnName("thread_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserAgent)
                .HasMaxLength(191)
                .HasColumnName("user_agent");
        });

        modelBuilder.Entity<Asset>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("assets")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => new { e.AssignedType, e.AssignedTo }, "assets_assigned_type_assigned_to_index");

            entity.HasIndex(e => e.CompanyId, "assets_company_id_index");

            entity.HasIndex(e => e.CreatedAt, "assets_created_at_index");

            entity.HasIndex(e => new { e.DeletedAt, e.AssetTag }, "assets_deleted_at_asset_tag_index");

            entity.HasIndex(e => new { e.DeletedAt, e.AssignedType, e.AssignedTo }, "assets_deleted_at_assigned_type_assigned_to_index");

            entity.HasIndex(e => new { e.DeletedAt, e.LocationId }, "assets_deleted_at_location_id_index");

            entity.HasIndex(e => new { e.DeletedAt, e.ModelId }, "assets_deleted_at_model_id_index");

            entity.HasIndex(e => new { e.DeletedAt, e.Name }, "assets_deleted_at_name_index");

            entity.HasIndex(e => new { e.DeletedAt, e.RtdLocationId }, "assets_deleted_at_rtd_location_id_index");

            entity.HasIndex(e => new { e.DeletedAt, e.StatusId }, "assets_deleted_at_status_id_index");

            entity.HasIndex(e => new { e.DeletedAt, e.SupplierId }, "assets_deleted_at_supplier_id_index");

            entity.HasIndex(e => e.RtdLocationId, "assets_rtd_location_id_index");

            entity.HasIndex(e => e.Serial, "assets_serial_index");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.Accepted)
                .HasMaxLength(191)
                .HasColumnName("accepted");
            entity.Property(e => e.Archived)
                .HasDefaultValueSql("'0'")
                .HasColumnName("archived");
            entity.Property(e => e.AssetEolDate).HasColumnName("asset_eol_date");
            entity.Property(e => e.AssetTag)
                .HasMaxLength(191)
                .HasColumnName("asset_tag");
            entity.Property(e => e.AssignedTo)
                .HasColumnType("int(11)")
                .HasColumnName("assigned_to");
            entity.Property(e => e.AssignedType)
                .HasMaxLength(191)
                .HasColumnName("assigned_type");
            entity.Property(e => e.Byod)
                .HasDefaultValueSql("'0'")
                .HasColumnName("byod");
            entity.Property(e => e.CheckinCounter)
                .HasColumnType("int(11)")
                .HasColumnName("checkin_counter");
            entity.Property(e => e.CheckoutCounter)
                .HasColumnType("int(11)")
                .HasColumnName("checkout_counter");
            entity.Property(e => e.CompanyId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasColumnType("int(11)")
                .HasColumnName("created_by");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Depreciate).HasColumnName("depreciate");
            entity.Property(e => e.EolExplicit).HasColumnName("eol_explicit");
            entity.Property(e => e.ExpectedCheckin).HasColumnName("expected_checkin");
            entity.Property(e => e.Image)
                .HasColumnType("text")
                .HasColumnName("image");
            entity.Property(e => e.LastAuditDate)
                .HasColumnType("datetime")
                .HasColumnName("last_audit_date");
            entity.Property(e => e.LastCheckin)
                .HasColumnType("datetime")
                .HasColumnName("last_checkin");
            entity.Property(e => e.LastCheckout)
                .HasColumnType("datetime")
                .HasColumnName("last_checkout");
            entity.Property(e => e.LocationId)
                .HasColumnType("int(11)")
                .HasColumnName("location_id");
            entity.Property(e => e.ModelId)
                .HasColumnType("int(11)")
                .HasColumnName("model_id");
            entity.Property(e => e.Name)
                .HasMaxLength(191)
                .HasColumnName("name");
            entity.Property(e => e.NextAuditDate).HasColumnName("next_audit_date");
            entity.Property(e => e.Notes)
                .HasColumnType("text")
                .HasColumnName("notes");
            entity.Property(e => e.OrderNumber)
                .HasMaxLength(191)
                .HasColumnName("order_number");
            entity.Property(e => e.Physical)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("physical");
            entity.Property(e => e.PurchaseCost)
                .HasPrecision(20, 2)
                .HasColumnName("purchase_cost");
            entity.Property(e => e.PurchaseDate).HasColumnName("purchase_date");
            entity.Property(e => e.Requestable)
                .HasColumnType("tinyint(4)")
                .HasColumnName("requestable");
            entity.Property(e => e.RequestsCounter)
                .HasColumnType("int(11)")
                .HasColumnName("requests_counter");
            entity.Property(e => e.RtdLocationId)
                .HasColumnType("int(11)")
                .HasColumnName("rtd_location_id");
            entity.Property(e => e.Serial)
                .HasMaxLength(191)
                .HasColumnName("serial");
            entity.Property(e => e.SnipeitMacAddress1)
                .HasMaxLength(191)
                .HasColumnName("_snipeit_mac_address_1");
            entity.Property(e => e.StatusId)
                .HasColumnType("int(11)")
                .HasColumnName("status_id");
            entity.Property(e => e.SupplierId)
                .HasColumnType("int(11)")
                .HasColumnName("supplier_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.WarrantyMonths)
                .HasColumnType("int(11)")
                .HasColumnName("warranty_months");
        });

        modelBuilder.Entity<AssetLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("asset_logs")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.AcceptedAt)
                .HasColumnType("datetime")
                .HasColumnName("accepted_at");
            entity.Property(e => e.AcceptedId)
                .HasColumnType("int(11)")
                .HasColumnName("accepted_id");
            entity.Property(e => e.AccessoryId)
                .HasColumnType("int(11)")
                .HasColumnName("accessory_id");
            entity.Property(e => e.ActionType)
                .HasMaxLength(191)
                .HasColumnName("action_type");
            entity.Property(e => e.AssetId)
                .HasColumnType("int(11)")
                .HasColumnName("asset_id");
            entity.Property(e => e.AssetType)
                .HasMaxLength(100)
                .HasColumnName("asset_type");
            entity.Property(e => e.CheckedoutTo)
                .HasColumnType("int(11)")
                .HasColumnName("checkedout_to");
            entity.Property(e => e.ComponentId)
                .HasColumnType("int(11)")
                .HasColumnName("component_id");
            entity.Property(e => e.ConsumableId)
                .HasColumnType("int(11)")
                .HasColumnName("consumable_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp")
                .HasColumnName("deleted_at");
            entity.Property(e => e.ExpectedCheckin).HasColumnName("expected_checkin");
            entity.Property(e => e.Filename)
                .HasColumnType("text")
                .HasColumnName("filename");
            entity.Property(e => e.LocationId)
                .HasColumnType("int(11)")
                .HasColumnName("location_id");
            entity.Property(e => e.Note)
                .HasColumnType("text")
                .HasColumnName("note");
            entity.Property(e => e.RequestedAt)
                .HasColumnType("datetime")
                .HasColumnName("requested_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId)
                .HasColumnType("int(11)")
                .HasColumnName("user_id");
        });

        modelBuilder.Entity<AssetMaintenance>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("asset_maintenances")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.AssetId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("asset_id");
            entity.Property(e => e.AssetMaintenanceTime)
                .HasColumnType("int(11)")
                .HasColumnName("asset_maintenance_time");
            entity.Property(e => e.AssetMaintenanceType)
                .HasMaxLength(191)
                .HasColumnName("asset_maintenance_type");
            entity.Property(e => e.CompletionDate).HasColumnName("completion_date");
            entity.Property(e => e.Cost)
                .HasPrecision(20, 2)
                .HasColumnName("cost");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasColumnType("int(11)")
                .HasColumnName("created_by");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");
            entity.Property(e => e.IsWarranty).HasColumnName("is_warranty");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.SupplierId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("supplier_id");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<AssetUpload>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("asset_uploads")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.AssetId)
                .HasColumnType("int(11)")
                .HasColumnName("asset_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Filename)
                .HasMaxLength(191)
                .HasColumnName("filename");
            entity.Property(e => e.Filenotes)
                .HasMaxLength(191)
                .HasColumnName("filenotes");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId)
                .HasColumnType("int(11)")
                .HasColumnName("user_id");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("categories")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.CategoryType)
                .HasMaxLength(191)
                .HasDefaultValueSql("'asset'")
                .HasColumnName("category_type");
            entity.Property(e => e.CheckinEmail).HasColumnName("checkin_email");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasColumnType("int(11)")
                .HasColumnName("created_by");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp")
                .HasColumnName("deleted_at");
            entity.Property(e => e.EulaText).HasColumnName("eula_text");
            entity.Property(e => e.Image)
                .HasMaxLength(191)
                .HasColumnName("image");
            entity.Property(e => e.Name)
                .HasMaxLength(191)
                .HasColumnName("name");
            entity.Property(e => e.RequireAcceptance).HasColumnName("require_acceptance");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.UseDefaultEula).HasColumnName("use_default_eula");
        });

        modelBuilder.Entity<CheckoutAcceptance>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("checkout_acceptances")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => new { e.CheckoutableType, e.CheckoutableId }, "checkout_acceptances_checkoutable_type_checkoutable_id_index");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.AcceptedAt)
                .HasColumnType("timestamp")
                .HasColumnName("accepted_at");
            entity.Property(e => e.AssignedToId)
                .HasColumnType("int(11)")
                .HasColumnName("assigned_to_id");
            entity.Property(e => e.CheckoutableId)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("checkoutable_id");
            entity.Property(e => e.CheckoutableType)
                .HasMaxLength(191)
                .HasColumnName("checkoutable_type");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.DeclinedAt)
                .HasColumnType("timestamp")
                .HasColumnName("declined_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Note)
                .HasColumnType("text")
                .HasColumnName("note");
            entity.Property(e => e.SignatureFilename)
                .HasMaxLength(191)
                .HasColumnName("signature_filename");
            entity.Property(e => e.StoredEula)
                .HasColumnType("text")
                .HasColumnName("stored_eula");
            entity.Property(e => e.StoredEulaFile)
                .HasMaxLength(191)
                .HasColumnName("stored_eula_file");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<CheckoutRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("checkout_requests")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => new { e.UserId, e.RequestableId, e.RequestableType }, "checkout_requests_user_id_requestable_id_requestable_type");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.CanceledAt)
                .HasColumnType("datetime")
                .HasColumnName("canceled_at");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");
            entity.Property(e => e.FulfilledAt)
                .HasColumnType("datetime")
                .HasColumnName("fulfilled_at");
            entity.Property(e => e.Quantity)
                .HasDefaultValueSql("'1'")
                .HasColumnType("int(11)")
                .HasColumnName("quantity");
            entity.Property(e => e.RequestableId)
                .HasColumnType("int(11)")
                .HasColumnName("requestable_id");
            entity.Property(e => e.RequestableType)
                .HasMaxLength(191)
                .HasColumnName("requestable_type");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId)
                .HasColumnType("int(11)")
                .HasColumnName("user_id");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("companies")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.Name, "companies_name_unique").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("created_by");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.Fax)
                .HasMaxLength(20)
                .HasColumnName("fax");
            entity.Property(e => e.Image)
                .HasMaxLength(191)
                .HasColumnName("image");
            entity.Property(e => e.Name)
                .HasMaxLength(191)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Component>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("components")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.CompanyId, "components_company_id_index");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.CategoryId)
                .HasColumnType("int(11)")
                .HasColumnName("category_id");
            entity.Property(e => e.CompanyId)
                .HasColumnType("int(11)")
                .HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasColumnType("int(11)")
                .HasColumnName("created_by");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Image)
                .HasMaxLength(191)
                .HasColumnName("image");
            entity.Property(e => e.LocationId)
                .HasColumnType("int(11)")
                .HasColumnName("location_id");
            entity.Property(e => e.ManufacturerId)
                .HasColumnType("int(11)")
                .HasColumnName("manufacturer_id");
            entity.Property(e => e.MinAmt)
                .HasColumnType("int(11)")
                .HasColumnName("min_amt");
            entity.Property(e => e.ModelNumber)
                .HasMaxLength(191)
                .HasColumnName("model_number");
            entity.Property(e => e.Name)
                .HasMaxLength(191)
                .HasColumnName("name");
            entity.Property(e => e.Notes)
                .HasColumnType("text")
                .HasColumnName("notes");
            entity.Property(e => e.OrderNumber)
                .HasMaxLength(191)
                .HasColumnName("order_number");
            entity.Property(e => e.PurchaseCost)
                .HasPrecision(20, 2)
                .HasColumnName("purchase_cost");
            entity.Property(e => e.PurchaseDate).HasColumnName("purchase_date");
            entity.Property(e => e.Qty)
                .HasDefaultValueSql("'1'")
                .HasColumnType("int(11)")
                .HasColumnName("qty");
            entity.Property(e => e.Serial)
                .HasMaxLength(191)
                .HasColumnName("serial");
            entity.Property(e => e.SupplierId)
                .HasColumnType("int(11)")
                .HasColumnName("supplier_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<ComponentsAsset>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("components_assets")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.AssetId)
                .HasColumnType("int(11)")
                .HasColumnName("asset_id");
            entity.Property(e => e.AssignedQty)
                .HasDefaultValueSql("'1'")
                .HasColumnType("int(11)")
                .HasColumnName("assigned_qty");
            entity.Property(e => e.ComponentId)
                .HasColumnType("int(11)")
                .HasColumnName("component_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasColumnType("int(11)")
                .HasColumnName("created_by");
            entity.Property(e => e.Note)
                .HasColumnType("text")
                .HasColumnName("note");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Consumable>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("consumables")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.CompanyId, "consumables_company_id_index");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.CategoryId)
                .HasColumnType("int(11)")
                .HasColumnName("category_id");
            entity.Property(e => e.CompanyId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasColumnType("int(11)")
                .HasColumnName("created_by");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Image)
                .HasMaxLength(191)
                .HasColumnName("image");
            entity.Property(e => e.ItemNo)
                .HasMaxLength(191)
                .HasColumnName("item_no");
            entity.Property(e => e.LocationId)
                .HasColumnType("int(11)")
                .HasColumnName("location_id");
            entity.Property(e => e.ManufacturerId)
                .HasColumnType("int(11)")
                .HasColumnName("manufacturer_id");
            entity.Property(e => e.MinAmt)
                .HasColumnType("int(11)")
                .HasColumnName("min_amt");
            entity.Property(e => e.ModelNumber)
                .HasMaxLength(191)
                .HasColumnName("model_number");
            entity.Property(e => e.Name)
                .HasMaxLength(191)
                .HasColumnName("name");
            entity.Property(e => e.Notes)
                .HasColumnType("text")
                .HasColumnName("notes");
            entity.Property(e => e.OrderNumber)
                .HasMaxLength(191)
                .HasColumnName("order_number");
            entity.Property(e => e.PurchaseCost)
                .HasPrecision(20, 2)
                .HasColumnName("purchase_cost");
            entity.Property(e => e.PurchaseDate).HasColumnName("purchase_date");
            entity.Property(e => e.Qty)
                .HasColumnType("int(11)")
                .HasColumnName("qty");
            entity.Property(e => e.Requestable).HasColumnName("requestable");
            entity.Property(e => e.SupplierId)
                .HasColumnType("int(11)")
                .HasColumnName("supplier_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<ConsumablesUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("consumables_users")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.AssignedTo)
                .HasColumnType("int(11)")
                .HasColumnName("assigned_to");
            entity.Property(e => e.ConsumableId)
                .HasColumnType("int(11)")
                .HasColumnName("consumable_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasColumnType("int(11)")
                .HasColumnName("created_by");
            entity.Property(e => e.Note)
                .HasColumnType("text")
                .HasColumnName("note");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<CustomField>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("custom_fields")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.AutoAddToFieldsets)
                .HasDefaultValueSql("'0'")
                .HasColumnName("auto_add_to_fieldsets");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasColumnType("int(11)")
                .HasColumnName("created_by");
            entity.Property(e => e.DbColumn)
                .HasMaxLength(191)
                .HasColumnName("db_column");
            entity.Property(e => e.DisplayInUserView)
                .HasDefaultValueSql("'0'")
                .HasColumnName("display_in_user_view");
            entity.Property(e => e.Element)
                .HasMaxLength(191)
                .HasColumnName("element");
            entity.Property(e => e.FieldEncrypted).HasColumnName("field_encrypted");
            entity.Property(e => e.FieldValues)
                .HasColumnType("text")
                .HasColumnName("field_values");
            entity.Property(e => e.Format)
                .HasMaxLength(191)
                .HasColumnName("format");
            entity.Property(e => e.HelpText)
                .HasColumnType("text")
                .HasColumnName("help_text");
            entity.Property(e => e.IsUnique)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_unique");
            entity.Property(e => e.Name)
                .HasMaxLength(191)
                .HasColumnName("name");
            entity.Property(e => e.ShowInEmail).HasColumnName("show_in_email");
            entity.Property(e => e.ShowInListview)
                .HasDefaultValueSql("'0'")
                .HasColumnName("show_in_listview");
            entity.Property(e => e.ShowInRequestableList)
                .HasDefaultValueSql("'0'")
                .HasColumnName("show_in_requestable_list");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<CustomFieldCustomFieldset>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("custom_field_custom_fieldset")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.CustomFieldId)
                .HasColumnType("int(11)")
                .HasColumnName("custom_field_id");
            entity.Property(e => e.CustomFieldsetId)
                .HasColumnType("int(11)")
                .HasColumnName("custom_fieldset_id");
            entity.Property(e => e.Order)
                .HasColumnType("int(11)")
                .HasColumnName("order");
            entity.Property(e => e.Required).HasColumnName("required");
        });

        modelBuilder.Entity<CustomFieldset>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("custom_fieldsets")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasColumnType("int(11)")
                .HasColumnName("created_by");
            entity.Property(e => e.Name)
                .HasMaxLength(191)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("departments")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.CompanyId, "departments_company_id_index");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.CompanyId)
                .HasColumnType("int(11)")
                .HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasColumnType("int(11)")
                .HasColumnName("created_by");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Fax)
                .HasMaxLength(20)
                .HasColumnName("fax");
            entity.Property(e => e.Image)
                .HasMaxLength(191)
                .HasColumnName("image");
            entity.Property(e => e.LocationId)
                .HasColumnType("int(11)")
                .HasColumnName("location_id");
            entity.Property(e => e.ManagerId)
                .HasColumnType("int(11)")
                .HasColumnName("manager_id");
            entity.Property(e => e.Name)
                .HasMaxLength(191)
                .HasColumnName("name");
            entity.Property(e => e.Notes)
                .HasMaxLength(191)
                .HasColumnName("notes");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Depreciation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("depreciations")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasColumnType("int(11)")
                .HasColumnName("created_by");
            entity.Property(e => e.DepreciationMin)
                .HasPrecision(8, 2)
                .HasColumnName("depreciation_min");
            entity.Property(e => e.DepreciationType)
                .HasMaxLength(191)
                .HasDefaultValueSql("'amount'")
                .HasColumnName("depreciation_type");
            entity.Property(e => e.Months)
                .HasColumnType("int(11)")
                .HasColumnName("months");
            entity.Property(e => e.Name)
                .HasMaxLength(191)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Import>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("imports")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("created_by");
            entity.Property(e => e.FieldMap)
                .HasColumnType("text")
                .HasColumnName("field_map");
            entity.Property(e => e.FilePath)
                .HasMaxLength(191)
                .HasColumnName("file_path");
            entity.Property(e => e.Filesize)
                .HasColumnType("int(11)")
                .HasColumnName("filesize");
            entity.Property(e => e.FirstRow)
                .HasColumnType("text")
                .HasColumnName("first_row");
            entity.Property(e => e.HeaderRow)
                .HasColumnType("text")
                .HasColumnName("header_row");
            entity.Property(e => e.ImportType)
                .HasMaxLength(191)
                .HasColumnName("import_type");
            entity.Property(e => e.Name)
                .HasMaxLength(191)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Kit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("kits")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("created_by");
            entity.Property(e => e.Name)
                .HasMaxLength(191)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<KitsAccessory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("kits_accessories")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.AccessoryId)
                .HasColumnType("int(11)")
                .HasColumnName("accessory_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("created_by");
            entity.Property(e => e.KitId)
                .HasColumnType("int(11)")
                .HasColumnName("kit_id");
            entity.Property(e => e.Quantity)
                .HasDefaultValueSql("'1'")
                .HasColumnType("int(11)")
                .HasColumnName("quantity");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<KitsConsumable>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("kits_consumables")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.ConsumableId)
                .HasColumnType("int(11)")
                .HasColumnName("consumable_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("created_by");
            entity.Property(e => e.KitId)
                .HasColumnType("int(11)")
                .HasColumnName("kit_id");
            entity.Property(e => e.Quantity)
                .HasDefaultValueSql("'1'")
                .HasColumnType("int(11)")
                .HasColumnName("quantity");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<KitsLicense>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("kits_licenses")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("created_by");
            entity.Property(e => e.KitId)
                .HasColumnType("int(11)")
                .HasColumnName("kit_id");
            entity.Property(e => e.LicenseId)
                .HasColumnType("int(11)")
                .HasColumnName("license_id");
            entity.Property(e => e.Quantity)
                .HasDefaultValueSql("'1'")
                .HasColumnType("int(11)")
                .HasColumnName("quantity");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<KitsModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("kits_models")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("created_by");
            entity.Property(e => e.KitId)
                .HasColumnType("int(11)")
                .HasColumnName("kit_id");
            entity.Property(e => e.ModelId)
                .HasColumnType("int(11)")
                .HasColumnName("model_id");
            entity.Property(e => e.Quantity)
                .HasDefaultValueSql("'1'")
                .HasColumnType("int(11)")
                .HasColumnName("quantity");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<License>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("licenses")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.CompanyId, "licenses_company_id_index");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.CategoryId)
                .HasColumnType("int(11)")
                .HasColumnName("category_id");
            entity.Property(e => e.CompanyId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasColumnType("int(11)")
                .HasColumnName("created_by");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Depreciate).HasColumnName("depreciate");
            entity.Property(e => e.DepreciationId)
                .HasColumnType("int(11)")
                .HasColumnName("depreciation_id");
            entity.Property(e => e.ExpirationDate).HasColumnName("expiration_date");
            entity.Property(e => e.LicenseEmail)
                .HasMaxLength(191)
                .HasColumnName("license_email");
            entity.Property(e => e.LicenseName)
                .HasMaxLength(120)
                .HasColumnName("license_name");
            entity.Property(e => e.Maintained).HasColumnName("maintained");
            entity.Property(e => e.ManufacturerId)
                .HasColumnType("int(11)")
                .HasColumnName("manufacturer_id");
            entity.Property(e => e.MinAmt)
                .HasColumnType("int(11)")
                .HasColumnName("min_amt");
            entity.Property(e => e.Name)
                .HasMaxLength(191)
                .HasColumnName("name");
            entity.Property(e => e.Notes)
                .HasColumnType("text")
                .HasColumnName("notes");
            entity.Property(e => e.OrderNumber)
                .HasMaxLength(50)
                .HasColumnName("order_number");
            entity.Property(e => e.PurchaseCost)
                .HasPrecision(20, 2)
                .HasColumnName("purchase_cost");
            entity.Property(e => e.PurchaseDate).HasColumnName("purchase_date");
            entity.Property(e => e.PurchaseOrder)
                .HasMaxLength(191)
                .HasColumnName("purchase_order");
            entity.Property(e => e.Reassignable)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("reassignable");
            entity.Property(e => e.Seats)
                .HasDefaultValueSql("'1'")
                .HasColumnType("int(11)")
                .HasColumnName("seats");
            entity.Property(e => e.Serial)
                .HasColumnType("text")
                .HasColumnName("serial");
            entity.Property(e => e.SupplierId)
                .HasColumnType("int(11)")
                .HasColumnName("supplier_id");
            entity.Property(e => e.TerminationDate).HasColumnName("termination_date");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<LicenseSeat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("license_seats")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => new { e.AssetId, e.LicenseId }, "license_seats_asset_id_license_id_index");

            entity.HasIndex(e => new { e.AssignedTo, e.LicenseId }, "license_seats_assigned_to_license_id_index");

            entity.HasIndex(e => e.LicenseId, "license_seats_license_id_index");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.AssetId)
                .HasColumnType("int(11)")
                .HasColumnName("asset_id");
            entity.Property(e => e.AssignedTo)
                .HasColumnType("int(11)")
                .HasColumnName("assigned_to");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasColumnType("int(11)")
                .HasColumnName("created_by");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp")
                .HasColumnName("deleted_at");
            entity.Property(e => e.LicenseId)
                .HasColumnType("int(11)")
                .HasColumnName("license_id");
            entity.Property(e => e.Notes)
                .HasColumnType("text")
                .HasColumnName("notes");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("locations")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => new { e.ManagerId, e.DeletedAt }, "locations_manager_id_deleted_at_index");

            entity.HasIndex(e => e.ParentId, "locations_parent_id_index");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(191)
                .HasColumnName("address");
            entity.Property(e => e.Address2)
                .HasMaxLength(191)
                .HasColumnName("address2");
            entity.Property(e => e.City)
                .HasMaxLength(191)
                .HasColumnName("city");
            entity.Property(e => e.Country)
                .HasMaxLength(191)
                .HasColumnName("country");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasColumnType("int(11)")
                .HasColumnName("created_by");
            entity.Property(e => e.Currency)
                .HasMaxLength(10)
                .HasColumnName("currency");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Fax)
                .HasMaxLength(20)
                .HasColumnName("fax");
            entity.Property(e => e.Image)
                .HasMaxLength(191)
                .HasColumnName("image");
            entity.Property(e => e.LdapOu)
                .HasMaxLength(191)
                .HasColumnName("ldap_ou");
            entity.Property(e => e.ManagerId)
                .HasColumnType("int(11)")
                .HasColumnName("manager_id");
            entity.Property(e => e.Name)
                .HasMaxLength(191)
                .HasColumnName("name");
            entity.Property(e => e.ParentId)
                .HasColumnType("int(11)")
                .HasColumnName("parent_id");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.State)
                .HasMaxLength(191)
                .HasColumnName("state");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.Zip)
                .HasMaxLength(10)
                .HasColumnName("zip");
        });

        modelBuilder.Entity<LoginAttempt>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("login_attempts")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.RemoteIp)
                .HasMaxLength(45)
                .HasColumnName("remote_ip");
            entity.Property(e => e.Successful).HasColumnName("successful");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserAgent)
                .HasMaxLength(191)
                .HasColumnName("user_agent");
            entity.Property(e => e.Username)
                .HasMaxLength(191)
                .HasColumnName("username");
        });

        modelBuilder.Entity<Manufacturer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("manufacturers")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasColumnType("int(11)")
                .HasColumnName("created_by");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Image)
                .HasMaxLength(191)
                .HasColumnName("image");
            entity.Property(e => e.Name)
                .HasMaxLength(191)
                .HasColumnName("name");
            entity.Property(e => e.SupportEmail)
                .HasMaxLength(191)
                .HasColumnName("support_email");
            entity.Property(e => e.SupportPhone)
                .HasMaxLength(191)
                .HasColumnName("support_phone");
            entity.Property(e => e.SupportUrl)
                .HasMaxLength(191)
                .HasColumnName("support_url");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.Url)
                .HasMaxLength(191)
                .HasColumnName("url");
            entity.Property(e => e.WarrantyLookupUrl)
                .HasMaxLength(191)
                .HasColumnName("warranty_lookup_url");
        });

        modelBuilder.Entity<Migration>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("migrations")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.Batch)
                .HasColumnType("int(11)")
                .HasColumnName("batch");
            entity.Property(e => e.Migration1)
                .HasMaxLength(191)
                .HasColumnName("migration");
        });

        modelBuilder.Entity<Model>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("models")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.CategoryId)
                .HasColumnType("int(11)")
                .HasColumnName("category_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasColumnType("int(11)")
                .HasColumnName("created_by");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp")
                .HasColumnName("deleted_at");
            entity.Property(e => e.DeprecatedMacAddress).HasColumnName("deprecated_mac_address");
            entity.Property(e => e.DepreciationId)
                .HasColumnType("int(11)")
                .HasColumnName("depreciation_id");
            entity.Property(e => e.Eol)
                .HasColumnType("int(11)")
                .HasColumnName("eol");
            entity.Property(e => e.FieldsetId)
                .HasColumnType("int(11)")
                .HasColumnName("fieldset_id");
            entity.Property(e => e.Image)
                .HasMaxLength(191)
                .HasColumnName("image");
            entity.Property(e => e.ManufacturerId)
                .HasColumnType("int(11)")
                .HasColumnName("manufacturer_id");
            entity.Property(e => e.MinAmt)
                .HasColumnType("int(11)")
                .HasColumnName("min_amt");
            entity.Property(e => e.ModelNumber)
                .HasMaxLength(191)
                .HasColumnName("model_number");
            entity.Property(e => e.Name)
                .HasMaxLength(191)
                .HasColumnName("name");
            entity.Property(e => e.Notes)
                .HasColumnType("text")
                .HasColumnName("notes");
            entity.Property(e => e.Requestable)
                .HasColumnType("tinyint(4)")
                .HasColumnName("requestable");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<ModelsCustomField>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("models_custom_fields")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.AssetModelId)
                .HasColumnType("int(11)")
                .HasColumnName("asset_model_id");
            entity.Property(e => e.CustomFieldId)
                .HasColumnType("int(11)")
                .HasColumnName("custom_field_id");
            entity.Property(e => e.DefaultValue)
                .HasColumnType("text")
                .HasColumnName("default_value");
        });

        modelBuilder.Entity<OauthAccessToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("oauth_access_tokens")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.UserId, "oauth_access_tokens_user_id_index");

            entity.Property(e => e.Id)
                .HasMaxLength(100)
                .HasColumnName("id");
            entity.Property(e => e.ClientId)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("client_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.ExpiresAt)
                .HasColumnType("datetime")
                .HasColumnName("expires_at");
            entity.Property(e => e.Name)
                .HasMaxLength(191)
                .HasColumnName("name");
            entity.Property(e => e.Revoked).HasColumnName("revoked");
            entity.Property(e => e.Scopes)
                .HasColumnType("text")
                .HasColumnName("scopes");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("user_id");
        });

        modelBuilder.Entity<OauthAuthCode>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("oauth_auth_codes")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.UserId, "oauth_auth_codes_user_id_index");

            entity.Property(e => e.Id)
                .HasMaxLength(100)
                .HasColumnName("id");
            entity.Property(e => e.ClientId)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("client_id");
            entity.Property(e => e.ExpiresAt)
                .HasColumnType("datetime")
                .HasColumnName("expires_at");
            entity.Property(e => e.Revoked).HasColumnName("revoked");
            entity.Property(e => e.Scopes)
                .HasColumnType("text")
                .HasColumnName("scopes");
            entity.Property(e => e.UserId)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("user_id");
        });

        modelBuilder.Entity<OauthClient>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("oauth_clients")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.UserId, "oauth_clients_user_id_index");

            entity.Property(e => e.Id)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasMaxLength(191)
                .HasColumnName("name");
            entity.Property(e => e.PasswordClient).HasColumnName("password_client");
            entity.Property(e => e.PersonalAccessClient).HasColumnName("personal_access_client");
            entity.Property(e => e.Provider)
                .HasMaxLength(191)
                .HasColumnName("provider");
            entity.Property(e => e.Redirect)
                .HasColumnType("text")
                .HasColumnName("redirect");
            entity.Property(e => e.Revoked).HasColumnName("revoked");
            entity.Property(e => e.Secret)
                .HasMaxLength(100)
                .HasColumnName("secret");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("user_id");
        });

        modelBuilder.Entity<OauthPersonalAccessClient>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("oauth_personal_access_clients")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.ClientId)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("client_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<OauthRefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("oauth_refresh_tokens")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.AccessTokenId, "oauth_refresh_tokens_access_token_id_index");

            entity.Property(e => e.Id)
                .HasMaxLength(100)
                .HasColumnName("id");
            entity.Property(e => e.AccessTokenId)
                .HasMaxLength(100)
                .HasColumnName("access_token_id");
            entity.Property(e => e.ExpiresAt)
                .HasColumnType("datetime")
                .HasColumnName("expires_at");
            entity.Property(e => e.Revoked).HasColumnName("revoked");
        });

        modelBuilder.Entity<PasswordReset>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("password_resets")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.Email, "password_resets_email_index");

            entity.HasIndex(e => e.Token, "password_resets_token_index");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(191)
                .HasColumnName("email");
            entity.Property(e => e.Token)
                .HasMaxLength(191)
                .HasColumnName("token");
        });

        modelBuilder.Entity<PermissionGroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("permission_groups")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasColumnType("int(11)")
                .HasColumnName("created_by");
            entity.Property(e => e.Name)
                .HasMaxLength(191)
                .HasColumnName("name");
            entity.Property(e => e.Permissions)
                .HasColumnType("text")
                .HasColumnName("permissions");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<ReportTemplate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("report_templates")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.CreatedBy, "report_templates_created_by_index");

            entity.Property(e => e.Id)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasColumnType("int(11)")
                .HasColumnName("created_by");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Name)
                .HasMaxLength(191)
                .HasColumnName("name");
            entity.Property(e => e.Options)
                .HasColumnType("text")
                .HasColumnName("options");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("requests")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.AssetId)
                .HasColumnType("int(11)")
                .HasColumnName("asset_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp")
                .HasColumnName("deleted_at");
            entity.Property(e => e.RequestCode)
                .HasColumnType("text")
                .HasColumnName("request_code");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId)
                .HasColumnType("int(11)")
                .HasColumnName("user_id");
        });

        modelBuilder.Entity<RequestedAsset>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("requested_assets")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.AcceptedAt)
                .HasColumnType("datetime")
                .HasColumnName("accepted_at");
            entity.Property(e => e.AssetId)
                .HasColumnType("int(11)")
                .HasColumnName("asset_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.DeniedAt)
                .HasColumnType("datetime")
                .HasColumnName("denied_at");
            entity.Property(e => e.Notes)
                .HasMaxLength(191)
                .HasColumnName("notes");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId)
                .HasColumnType("int(11)")
                .HasColumnName("user_id");
        });

        modelBuilder.Entity<SamlNonce>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("saml_nonces")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.Nonce, "saml_nonces_nonce_index");

            entity.HasIndex(e => e.NotValidAfter, "saml_nonces_not_valid_after_index");

            entity.Property(e => e.Id)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.Nonce)
                .HasMaxLength(191)
                .HasColumnName("nonce");
            entity.Property(e => e.NotValidAfter)
                .HasColumnType("datetime")
                .HasColumnName("not_valid_after");
        });

        modelBuilder.Entity<Setting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("settings")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.AdAppendDomain).HasColumnName("ad_append_domain");
            entity.Property(e => e.AdDomain)
                .HasMaxLength(191)
                .HasColumnName("ad_domain");
            entity.Property(e => e.AdminCcEmail)
                .HasMaxLength(191)
                .IsFixedLength()
                .HasColumnName("admin_cc_email");
            entity.Property(e => e.AlertEmail)
                .HasMaxLength(191)
                .HasColumnName("alert_email");
            entity.Property(e => e.AlertInterval)
                .HasDefaultValueSql("'30'")
                .HasColumnType("int(11)")
                .HasColumnName("alert_interval");
            entity.Property(e => e.AlertThreshold)
                .HasDefaultValueSql("'5'")
                .HasColumnType("int(11)")
                .HasColumnName("alert_threshold");
            entity.Property(e => e.AlertsEnabled)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("alerts_enabled");
            entity.Property(e => e.AllowUserSkin).HasColumnName("allow_user_skin");
            entity.Property(e => e.AltBarcodeEnabled)
                .HasDefaultValueSql("'1'")
                .HasColumnName("alt_barcode_enabled");
            entity.Property(e => e.AuditInterval)
                .HasColumnType("int(11)")
                .HasColumnName("audit_interval");
            entity.Property(e => e.AuditWarningDays)
                .HasColumnType("int(11)")
                .HasColumnName("audit_warning_days");
            entity.Property(e => e.AutoIncrementAssets)
                .HasColumnType("int(11)")
                .HasColumnName("auto_increment_assets");
            entity.Property(e => e.AutoIncrementPrefix)
                .HasMaxLength(191)
                .HasColumnName("auto_increment_prefix");
            entity.Property(e => e.Brand)
                .HasDefaultValueSql("'1'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("brand");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasColumnType("int(11)")
                .HasColumnName("created_by");
            entity.Property(e => e.CustomCss)
                .HasColumnType("text")
                .HasColumnName("custom_css");
            entity.Property(e => e.CustomForgotPassUrl)
                .HasMaxLength(191)
                .HasColumnName("custom_forgot_pass_url");
            entity.Property(e => e.DashChartType)
                .HasMaxLength(191)
                .HasDefaultValueSql("'name'")
                .HasColumnName("dash_chart_type");
            entity.Property(e => e.DashboardMessage)
                .HasColumnType("text")
                .HasColumnName("dashboard_message");
            entity.Property(e => e.DateDisplayFormat)
                .HasMaxLength(191)
                .HasDefaultValueSql("'Y-m-d'")
                .HasColumnName("date_display_format");
            entity.Property(e => e.DefaultAvatar)
                .HasMaxLength(191)
                .HasDefaultValueSql("'default.png'")
                .HasColumnName("default_avatar");
            entity.Property(e => e.DefaultCurrency)
                .HasMaxLength(10)
                .HasColumnName("default_currency");
            entity.Property(e => e.DefaultEulaText).HasColumnName("default_eula_text");
            entity.Property(e => e.DepreciationMethod)
                .HasMaxLength(10)
                .HasDefaultValueSql("'default'")
                .IsFixedLength()
                .HasColumnName("depreciation_method");
            entity.Property(e => e.DigitSeparator)
                .HasMaxLength(191)
                .HasDefaultValueSql("'1,234.56'")
                .IsFixedLength()
                .HasColumnName("digit_separator");
            entity.Property(e => e.DisplayAssetName)
                .HasColumnType("int(11)")
                .HasColumnName("display_asset_name");
            entity.Property(e => e.DisplayCheckoutDate)
                .HasColumnType("int(11)")
                .HasColumnName("display_checkout_date");
            entity.Property(e => e.DisplayEol)
                .HasColumnType("int(11)")
                .HasColumnName("display_eol");
            entity.Property(e => e.DueCheckinDays)
                .HasColumnType("int(11)")
                .HasColumnName("due_checkin_days");
            entity.Property(e => e.EmailDomain)
                .HasMaxLength(191)
                .HasColumnName("email_domain");
            entity.Property(e => e.EmailFormat)
                .HasMaxLength(191)
                .HasDefaultValueSql("'filastname'")
                .HasColumnName("email_format");
            entity.Property(e => e.EmailLogo)
                .HasMaxLength(191)
                .IsFixedLength()
                .HasColumnName("email_logo");
            entity.Property(e => e.Favicon)
                .HasMaxLength(191)
                .IsFixedLength()
                .HasColumnName("favicon");
            entity.Property(e => e.FooterText)
                .HasColumnType("text")
                .HasColumnName("footer_text");
            entity.Property(e => e.FullMultipleCompaniesSupport).HasColumnName("full_multiple_companies_support");
            entity.Property(e => e.GoogleClientId)
                .HasMaxLength(191)
                .HasColumnName("google_client_id");
            entity.Property(e => e.GoogleClientSecret)
                .HasMaxLength(191)
                .HasColumnName("google_client_secret");
            entity.Property(e => e.GoogleLogin)
                .HasDefaultValueSql("'0'")
                .HasColumnName("google_login");
            entity.Property(e => e.HeaderColor)
                .HasMaxLength(191)
                .HasColumnName("header_color");
            entity.Property(e => e.IsAd).HasColumnName("is_ad");
            entity.Property(e => e.Label21dType)
                .HasMaxLength(191)
                .HasDefaultValueSql("'C128'")
                .HasColumnName("label2_1d_type");
            entity.Property(e => e.Label22dTarget)
                .HasMaxLength(191)
                .HasDefaultValueSql("'hardware_id'")
                .HasColumnName("label2_2d_target");
            entity.Property(e => e.Label22dType)
                .HasMaxLength(191)
                .HasDefaultValueSql("'QRCODE'")
                .HasColumnName("label2_2d_type");
            entity.Property(e => e.Label2AssetLogo).HasColumnName("label2_asset_logo");
            entity.Property(e => e.Label2Enable).HasColumnName("label2_enable");
            entity.Property(e => e.Label2Fields)
                .HasMaxLength(191)
                .HasDefaultValueSql("'name=name;serial=serial;model=model.name;'")
                .HasColumnName("label2_fields");
            entity.Property(e => e.Label2Template)
                .HasMaxLength(191)
                .HasDefaultValueSql("'DefaultLabel'")
                .HasColumnName("label2_template");
            entity.Property(e => e.Label2Title)
                .HasMaxLength(191)
                .HasColumnName("label2_title");
            entity.Property(e => e.LabelLogo)
                .HasMaxLength(191)
                .IsFixedLength()
                .HasColumnName("label_logo");
            entity.Property(e => e.LabelsDisplayBgutter)
                .HasPrecision(6, 5)
                .HasDefaultValueSql("'0.07000'")
                .HasColumnName("labels_display_bgutter");
            entity.Property(e => e.LabelsDisplayCompanyName).HasColumnName("labels_display_company_name");
            entity.Property(e => e.LabelsDisplayModel).HasColumnName("labels_display_model");
            entity.Property(e => e.LabelsDisplayName)
                .HasColumnType("tinyint(4)")
                .HasColumnName("labels_display_name");
            entity.Property(e => e.LabelsDisplaySerial)
                .HasDefaultValueSql("'1'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("labels_display_serial");
            entity.Property(e => e.LabelsDisplaySgutter)
                .HasPrecision(6, 5)
                .HasDefaultValueSql("'0.05000'")
                .HasColumnName("labels_display_sgutter");
            entity.Property(e => e.LabelsDisplayTag)
                .HasDefaultValueSql("'1'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("labels_display_tag");
            entity.Property(e => e.LabelsFontsize)
                .HasDefaultValueSql("'9'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("labels_fontsize");
            entity.Property(e => e.LabelsHeight)
                .HasPrecision(6, 5)
                .HasDefaultValueSql("'1.00000'")
                .HasColumnName("labels_height");
            entity.Property(e => e.LabelsPageheight)
                .HasPrecision(7, 5)
                .HasDefaultValueSql("'11.00000'")
                .HasColumnName("labels_pageheight");
            entity.Property(e => e.LabelsPagewidth)
                .HasPrecision(7, 5)
                .HasDefaultValueSql("'8.50000'")
                .HasColumnName("labels_pagewidth");
            entity.Property(e => e.LabelsPerPage)
                .HasDefaultValueSql("'30'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("labels_per_page");
            entity.Property(e => e.LabelsPmarginBottom)
                .HasPrecision(6, 5)
                .HasDefaultValueSql("'0.50000'")
                .HasColumnName("labels_pmargin_bottom");
            entity.Property(e => e.LabelsPmarginLeft)
                .HasPrecision(6, 5)
                .HasDefaultValueSql("'0.21975'")
                .HasColumnName("labels_pmargin_left");
            entity.Property(e => e.LabelsPmarginRight)
                .HasPrecision(6, 5)
                .HasDefaultValueSql("'0.21975'")
                .HasColumnName("labels_pmargin_right");
            entity.Property(e => e.LabelsPmarginTop)
                .HasPrecision(6, 5)
                .HasDefaultValueSql("'0.50000'")
                .HasColumnName("labels_pmargin_top");
            entity.Property(e => e.LabelsWidth)
                .HasPrecision(6, 5)
                .HasDefaultValueSql("'2.62500'")
                .HasColumnName("labels_width");
            entity.Property(e => e.LdapActiveFlag)
                .HasMaxLength(191)
                .HasColumnName("ldap_active_flag");
            entity.Property(e => e.LdapAuthFilterQuery)
                .HasMaxLength(191)
                .HasDefaultValueSql("'uid='")
                .HasColumnName("ldap_auth_filter_query");
            entity.Property(e => e.LdapBasedn)
                .HasMaxLength(191)
                .HasColumnName("ldap_basedn");
            entity.Property(e => e.LdapClientTlsCert)
                .HasColumnType("text")
                .HasColumnName("ldap_client_tls_cert");
            entity.Property(e => e.LdapClientTlsKey)
                .HasColumnType("text")
                .HasColumnName("ldap_client_tls_key");
            entity.Property(e => e.LdapCountry)
                .HasMaxLength(191)
                .HasColumnName("ldap_country");
            entity.Property(e => e.LdapDefaultGroup)
                .HasMaxLength(191)
                .HasColumnName("ldap_default_group");
            entity.Property(e => e.LdapDept)
                .HasMaxLength(191)
                .HasColumnName("ldap_dept");
            entity.Property(e => e.LdapEmail)
                .HasMaxLength(191)
                .HasColumnName("ldap_email");
            entity.Property(e => e.LdapEmpNum)
                .HasMaxLength(191)
                .HasColumnName("ldap_emp_num");
            entity.Property(e => e.LdapEnabled)
                .HasMaxLength(191)
                .HasColumnName("ldap_enabled");
            entity.Property(e => e.LdapFilter)
                .HasColumnType("text")
                .HasColumnName("ldap_filter");
            entity.Property(e => e.LdapFnameField)
                .HasMaxLength(191)
                .HasDefaultValueSql("'givenname'")
                .HasColumnName("ldap_fname_field");
            entity.Property(e => e.LdapJobtitle)
                .HasMaxLength(191)
                .HasColumnName("ldap_jobtitle");
            entity.Property(e => e.LdapLnameField)
                .HasMaxLength(191)
                .HasDefaultValueSql("'sn'")
                .HasColumnName("ldap_lname_field");
            entity.Property(e => e.LdapLocation)
                .HasMaxLength(191)
                .HasColumnName("ldap_location");
            entity.Property(e => e.LdapManager)
                .HasMaxLength(191)
                .HasColumnName("ldap_manager");
            entity.Property(e => e.LdapPhoneField)
                .HasMaxLength(191)
                .HasColumnName("ldap_phone_field");
            entity.Property(e => e.LdapPort)
                .HasMaxLength(5)
                .HasDefaultValueSql("'389'")
                .HasColumnName("ldap_port");
            entity.Property(e => e.LdapPwSync)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("ldap_pw_sync");
            entity.Property(e => e.LdapPword).HasColumnName("ldap_pword");
            entity.Property(e => e.LdapServer)
                .HasMaxLength(191)
                .HasColumnName("ldap_server");
            entity.Property(e => e.LdapServerCertIgnore).HasColumnName("ldap_server_cert_ignore");
            entity.Property(e => e.LdapTls).HasColumnName("ldap_tls");
            entity.Property(e => e.LdapUname)
                .HasMaxLength(191)
                .HasColumnName("ldap_uname");
            entity.Property(e => e.LdapUsernameField)
                .HasMaxLength(191)
                .HasDefaultValueSql("'samaccountname'")
                .HasColumnName("ldap_username_field");
            entity.Property(e => e.LdapVersion)
                .HasDefaultValueSql("'3'")
                .HasColumnType("int(11)")
                .HasColumnName("ldap_version");
            entity.Property(e => e.LoadRemote)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("load_remote");
            entity.Property(e => e.Locale)
                .HasMaxLength(10)
                .HasDefaultValueSql("'en-US'")
                .HasColumnName("locale");
            entity.Property(e => e.LoginCommonDisabled).HasColumnName("login_common_disabled");
            entity.Property(e => e.LoginNote)
                .HasColumnType("text")
                .HasColumnName("login_note");
            entity.Property(e => e.LoginRemoteUserCustomLogoutUrl)
                .HasMaxLength(191)
                .HasDefaultValueSql("''")
                .HasColumnName("login_remote_user_custom_logout_url");
            entity.Property(e => e.LoginRemoteUserEnabled).HasColumnName("login_remote_user_enabled");
            entity.Property(e => e.LoginRemoteUserHeaderName)
                .HasMaxLength(191)
                .HasDefaultValueSql("''")
                .HasColumnName("login_remote_user_header_name");
            entity.Property(e => e.Logo)
                .HasMaxLength(191)
                .HasColumnName("logo");
            entity.Property(e => e.LogoPrintAssets).HasColumnName("logo_print_assets");
            entity.Property(e => e.ModellistDisplays)
                .HasMaxLength(191)
                .HasDefaultValueSql("'image,category,manufacturer,model_number'")
                .IsFixedLength()
                .HasColumnName("modellist_displays");
            entity.Property(e => e.NameDisplayFormat)
                .HasMaxLength(10)
                .HasDefaultValueSql("'first_last'")
                .HasColumnName("name_display_format");
            entity.Property(e => e.NextAutoTagBase)
                .HasDefaultValueSql("'1'")
                .HasColumnType("bigint(20)")
                .HasColumnName("next_auto_tag_base");
            entity.Property(e => e.PerPage)
                .HasDefaultValueSql("'20'")
                .HasColumnType("int(11)")
                .HasColumnName("per_page");
            entity.Property(e => e.PrivacyPolicyLink)
                .HasMaxLength(191)
                .IsFixedLength()
                .HasColumnName("privacy_policy_link");
            entity.Property(e => e.ProfileEdit)
                .HasDefaultValueSql("'1'")
                .HasColumnName("profile_edit");
            entity.Property(e => e.PwdSecureComplexity)
                .HasMaxLength(191)
                .HasColumnName("pwd_secure_complexity");
            entity.Property(e => e.PwdSecureMin)
                .HasDefaultValueSql("'8'")
                .HasColumnType("int(11)")
                .HasColumnName("pwd_secure_min");
            entity.Property(e => e.PwdSecureUncommon).HasColumnName("pwd_secure_uncommon");
            entity.Property(e => e.QrCode)
                .HasColumnType("int(11)")
                .HasColumnName("qr_code");
            entity.Property(e => e.QrText)
                .HasMaxLength(32)
                .HasColumnName("qr_text");
            entity.Property(e => e.RequireAcceptSignature).HasColumnName("require_accept_signature");
            entity.Property(e => e.RequireCheckinoutNotes)
                .HasDefaultValueSql("'0'")
                .HasColumnName("require_checkinout_notes");
            entity.Property(e => e.SamlAttrMappingUsername)
                .HasMaxLength(191)
                .HasColumnName("saml_attr_mapping_username");
            entity.Property(e => e.SamlCustomSettings)
                .HasColumnType("text")
                .HasColumnName("saml_custom_settings");
            entity.Property(e => e.SamlEnabled).HasColumnName("saml_enabled");
            entity.Property(e => e.SamlForcelogin).HasColumnName("saml_forcelogin");
            entity.Property(e => e.SamlIdpMetadata)
                .HasColumnType("mediumtext")
                .HasColumnName("saml_idp_metadata");
            entity.Property(e => e.SamlSlo).HasColumnName("saml_slo");
            entity.Property(e => e.SamlSpPrivatekey)
                .HasColumnType("text")
                .HasColumnName("saml_sp_privatekey");
            entity.Property(e => e.SamlSpX509cert)
                .HasColumnType("text")
                .HasColumnName("saml_sp_x509cert");
            entity.Property(e => e.SamlSpX509certNew)
                .HasColumnType("text")
                .HasColumnName("saml_sp_x509certNew");
            entity.Property(e => e.ShortcutsEnabled).HasColumnName("shortcuts_enabled");
            entity.Property(e => e.ShowAlertsInMenu)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("show_alerts_in_menu");
            entity.Property(e => e.ShowArchivedInList).HasColumnName("show_archived_in_list");
            entity.Property(e => e.ShowAssignedAssets).HasColumnName("show_assigned_assets");
            entity.Property(e => e.ShowImagesInEmail)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("show_images_in_email");
            entity.Property(e => e.ShowUrlInEmails).HasColumnName("show_url_in_emails");
            entity.Property(e => e.SiteName)
                .HasMaxLength(100)
                .HasDefaultValueSql("'Snipe IT Asset Management'")
                .HasColumnName("site_name");
            entity.Property(e => e.Skin)
                .HasMaxLength(191)
                .IsFixedLength()
                .HasColumnName("skin");
            entity.Property(e => e.SupportFooter)
                .HasMaxLength(5)
                .HasDefaultValueSql("'on'")
                .IsFixedLength()
                .HasColumnName("support_footer");
            entity.Property(e => e.ThumbnailMaxH)
                .HasDefaultValueSql("'50'")
                .HasColumnType("int(11)")
                .HasColumnName("thumbnail_max_h");
            entity.Property(e => e.TimeDisplayFormat)
                .HasMaxLength(191)
                .HasDefaultValueSql("'h:i A'")
                .HasColumnName("time_display_format");
            entity.Property(e => e.TwoFactorEnabled)
                .HasColumnType("tinyint(4)")
                .HasColumnName("two_factor_enabled");
            entity.Property(e => e.UniqueSerial).HasColumnName("unique_serial");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.UsernameFormat)
                .HasMaxLength(191)
                .HasDefaultValueSql("'filastname'")
                .HasColumnName("username_format");
            entity.Property(e => e.VersionFooter)
                .HasMaxLength(5)
                .HasDefaultValueSql("'on'")
                .IsFixedLength()
                .HasColumnName("version_footer");
            entity.Property(e => e.WebhookBotname)
                .HasMaxLength(191)
                .HasColumnName("webhook_botname");
            entity.Property(e => e.WebhookChannel)
                .HasMaxLength(191)
                .HasColumnName("webhook_channel");
            entity.Property(e => e.WebhookEndpoint)
                .HasColumnType("text")
                .HasColumnName("webhook_endpoint");
            entity.Property(e => e.WebhookSelected)
                .HasMaxLength(191)
                .HasDefaultValueSql("'slack'")
                .HasColumnName("webhook_selected");
            entity.Property(e => e.ZerofillCount)
                .HasDefaultValueSql("'5'")
                .HasColumnType("int(11)")
                .HasColumnName("zerofill_count");
        });

        modelBuilder.Entity<StatusLabel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("status_labels")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.Archived).HasColumnName("archived");
            entity.Property(e => e.Color)
                .HasMaxLength(10)
                .HasColumnName("color");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasColumnType("int(11)")
                .HasColumnName("created_by");
            entity.Property(e => e.DefaultLabel)
                .HasDefaultValueSql("'0'")
                .HasColumnName("default_label");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Deployable).HasColumnName("deployable");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Notes)
                .HasColumnType("text")
                .HasColumnName("notes");
            entity.Property(e => e.Pending).HasColumnName("pending");
            entity.Property(e => e.ShowInNav)
                .HasDefaultValueSql("'0'")
                .HasColumnName("show_in_nav");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("suppliers")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(250)
                .HasColumnName("address");
            entity.Property(e => e.Address2)
                .HasMaxLength(250)
                .HasColumnName("address2");
            entity.Property(e => e.City)
                .HasMaxLength(191)
                .HasColumnName("city");
            entity.Property(e => e.Contact)
                .HasMaxLength(100)
                .HasColumnName("contact");
            entity.Property(e => e.Country)
                .HasMaxLength(2)
                .HasColumnName("country");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasColumnType("int(11)")
                .HasColumnName("created_by");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.Fax)
                .HasMaxLength(35)
                .HasColumnName("fax");
            entity.Property(e => e.Image)
                .HasMaxLength(191)
                .HasColumnName("image");
            entity.Property(e => e.Name)
                .HasMaxLength(191)
                .HasColumnName("name");
            entity.Property(e => e.Notes)
                .HasMaxLength(191)
                .HasColumnName("notes");
            entity.Property(e => e.Phone)
                .HasMaxLength(35)
                .HasColumnName("phone");
            entity.Property(e => e.State)
                .HasMaxLength(191)
                .HasColumnName("state");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.Url)
                .HasMaxLength(250)
                .HasColumnName("url");
            entity.Property(e => e.Zip)
                .HasMaxLength(10)
                .HasColumnName("zip");
        });

        modelBuilder.Entity<Throttle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("throttle")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.UserId, "throttle_user_id_index");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.Attempts)
                .HasColumnType("int(11)")
                .HasColumnName("attempts");
            entity.Property(e => e.Banned).HasColumnName("banned");
            entity.Property(e => e.BannedAt)
                .HasColumnType("timestamp")
                .HasColumnName("banned_at");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(191)
                .HasColumnName("ip_address");
            entity.Property(e => e.LastAttemptAt)
                .HasColumnType("timestamp")
                .HasColumnName("last_attempt_at");
            entity.Property(e => e.Suspended).HasColumnName("suspended");
            entity.Property(e => e.SuspendedAt)
                .HasColumnType("timestamp")
                .HasColumnName("suspended_at");
            entity.Property(e => e.UserId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("user_id");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("users")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.ActivationCode, "users_activation_code_index");

            entity.HasIndex(e => e.CompanyId, "users_company_id_index");

            entity.HasIndex(e => new { e.ManagerId, e.DeletedAt }, "users_manager_id_deleted_at_index");

            entity.HasIndex(e => e.ResetPasswordCode, "users_reset_password_code_index");

            entity.HasIndex(e => new { e.Username, e.DeletedAt }, "users_username_deleted_at_index");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.Activated).HasColumnName("activated");
            entity.Property(e => e.ActivatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("activated_at");
            entity.Property(e => e.ActivationCode)
                .HasMaxLength(191)
                .HasColumnName("activation_code");
            entity.Property(e => e.Address)
                .HasMaxLength(191)
                .HasColumnName("address");
            entity.Property(e => e.AutoassignLicenses)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("autoassign_licenses");
            entity.Property(e => e.Avatar)
                .HasMaxLength(191)
                .HasColumnName("avatar");
            entity.Property(e => e.City)
                .HasMaxLength(191)
                .HasColumnName("city");
            entity.Property(e => e.CompanyId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("company_id");
            entity.Property(e => e.Country)
                .HasMaxLength(191)
                .HasColumnName("country");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasColumnType("int(11)")
                .HasColumnName("created_by");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp")
                .HasColumnName("deleted_at");
            entity.Property(e => e.DepartmentId)
                .HasColumnType("int(11)")
                .HasColumnName("department_id");
            entity.Property(e => e.Email)
                .HasMaxLength(191)
                .HasColumnName("email");
            entity.Property(e => e.EmployeeNum)
                .HasColumnType("text")
                .HasColumnName("employee_num");
            entity.Property(e => e.EnableConfetti).HasColumnName("enable_confetti");
            entity.Property(e => e.EnableSounds).HasColumnName("enable_sounds");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.FirstName)
                .HasMaxLength(191)
                .HasColumnName("first_name");
            entity.Property(e => e.Gravatar)
                .HasMaxLength(191)
                .HasColumnName("gravatar");
            entity.Property(e => e.Jobtitle)
                .HasMaxLength(191)
                .HasColumnName("jobtitle");
            entity.Property(e => e.LastLogin)
                .HasColumnType("timestamp")
                .HasColumnName("last_login");
            entity.Property(e => e.LastName)
                .HasMaxLength(191)
                .HasColumnName("last_name");
            entity.Property(e => e.LdapImport).HasColumnName("ldap_import");
            entity.Property(e => e.Locale)
                .HasMaxLength(10)
                .HasDefaultValueSql("'en-US'")
                .HasColumnName("locale");
            entity.Property(e => e.LocationId)
                .HasColumnType("int(11)")
                .HasColumnName("location_id");
            entity.Property(e => e.ManagerId)
                .HasColumnType("int(11)")
                .HasColumnName("manager_id");
            entity.Property(e => e.Notes)
                .HasColumnType("text")
                .HasColumnName("notes");
            entity.Property(e => e.Password)
                .HasMaxLength(191)
                .HasColumnName("password");
            entity.Property(e => e.Permissions)
                .HasColumnType("text")
                .HasColumnName("permissions");
            entity.Property(e => e.PersistCode)
                .HasMaxLength(191)
                .HasColumnName("persist_code");
            entity.Property(e => e.Phone)
                .HasMaxLength(191)
                .HasColumnName("phone");
            entity.Property(e => e.RememberToken)
                .HasColumnType("text")
                .HasColumnName("remember_token");
            entity.Property(e => e.Remote)
                .HasDefaultValueSql("'0'")
                .HasColumnName("remote");
            entity.Property(e => e.ResetPasswordCode)
                .HasMaxLength(191)
                .HasColumnName("reset_password_code");
            entity.Property(e => e.ScimExternalid)
                .HasMaxLength(191)
                .HasColumnName("scim_externalid");
            entity.Property(e => e.ShowInList)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("show_in_list");
            entity.Property(e => e.Skin)
                .HasMaxLength(191)
                .HasColumnName("skin");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.State)
                .HasMaxLength(191)
                .HasColumnName("state");
            entity.Property(e => e.TwoFactorEnrolled).HasColumnName("two_factor_enrolled");
            entity.Property(e => e.TwoFactorOptin).HasColumnName("two_factor_optin");
            entity.Property(e => e.TwoFactorSecret)
                .HasMaxLength(32)
                .HasColumnName("two_factor_secret");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.Username)
                .HasMaxLength(191)
                .HasColumnName("username");
            entity.Property(e => e.Vip)
                .HasDefaultValueSql("'0'")
                .HasColumnName("vip");
            entity.Property(e => e.Website)
                .HasMaxLength(191)
                .HasColumnName("website");
            entity.Property(e => e.Zip)
                .HasMaxLength(10)
                .HasColumnName("zip");
        });

        modelBuilder.Entity<UsersGroup>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.GroupId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity
                .ToTable("users_groups")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.UserId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("user_id");
            entity.Property(e => e.GroupId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("group_id");
            entity.Property(e => e.CreatedBy)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("created_by");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
