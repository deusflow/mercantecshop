# DTO Field Names Comparison with Models

## ❌ НЕСООТВЕТСТВИЯ НАЙДЕНЫ!

### 1. **UserDto** - МНОГО НЕСООТВЕТСТВИЙ

| DTO Field Name | Model Field Name | Status |
|---------------|-----------------|---------|
| ~~JobTitle~~ | **Jobtitle** | ❌ WRONG |
| ~~EmployeeNumber~~ | **EmployeeNum** | ❌ WRONG |
| ~~IsRemote~~ | **Remote** | ❌ WRONG |
| ~~IsVip~~ | **Vip** | ❌ WRONG |
| ~~IsActivated~~ | **Activated** | ❌ WRONG |
| ~~LastLoginAt~~ | **LastLogin** | ❌ WRONG |
| Username | Username | ✅ OK |
| Email | Email | ✅ OK |
| FirstName | FirstName | ✅ OK |
| LastName | LastName | ✅ OK |
| Avatar | Avatar | ✅ OK |
| Phone | Phone | ✅ OK |
| LocationId | LocationId | ✅ OK |
| DepartmentId | DepartmentId | ✅ OK |
| CompanyId | CompanyId | ✅ OK |
| ManagerId | ManagerId | ✅ OK |
| Address | Address | ✅ OK |
| City | City | ✅ OK |
| State | State | ✅ OK |
| Country | Country | ✅ OK |
| Zip | Zip | ✅ OK |
| StartDate | StartDate | ✅ OK |
| EndDate | EndDate | ✅ OK |
| Website | Website | ✅ OK |
| Notes | Notes | ✅ OK |
| CreatedAt | CreatedAt | ✅ OK |
| UpdatedAt | UpdatedAt | ✅ OK |

**ОТСУТСТВУЮТ в DTO (есть в модели):**
- Gravatar (profile image URL from Gravatar service)
- RememberToken
- LdapImport
- Locale
- TwoFactorSecret, TwoFactorEnrolled, TwoFactorOptin
- Skin
- ScimExternalid
- AutoassignLicenses
- EnableSounds, EnableConfetti

---

### 2. **ProductDto** (Asset model) - НЕСООТВЕТСТВИЯ

| DTO Field Name | Model Field Name | Status |
|---------------|-----------------|---------|
| ~~ModelName~~ | **ModelId** (reference) | ❌ WRONG (нужен ModelId) |
| AssetTag | AssetTag | ✅ OK |
| Name | Name | ✅ OK |
| Serial | Serial | ✅ OK |
| StatusId | StatusId | ✅ OK |
| Image | Image | ✅ OK |
| Notes | Notes | ✅ OK |
| LocationId | LocationId | ✅ OK |
| ManufacturerId | - | ❌ Через Model |
| PurchaseDate | PurchaseDate | ✅ OK |
| CreatedAt | CreatedAt | ✅ OK |
| UpdatedAt | UpdatedAt | ✅ OK |

**ОТСУТСТВУЮТ в DTO:**
- ModelId (ВАЖНО!)
- SupplierId
- OrderNumber
- PurchaseCost (вместо Price)
- WarrantyMonths
- RtdLocationId (Ready to Deploy Location)
- CompanyId
- AssignedTo
- AssignedType
- Requestable (sbyte в модели)

---

### 3. **CategoryDto** - OK, но можно улучшить

| DTO Field Name | Model Field Name | Status |
|---------------|-----------------|---------|
| ~~Type~~ | **CategoryType** | ❌ WRONG |
| Name | Name | ✅ OK |
| Image | Image | ✅ OK |
| CreatedAt | CreatedAt | ✅ OK |
| UpdatedAt | UpdatedAt | ✅ OK |

**ОТСУТСТВУЮТ в DTO:**
- EulaText
- UseDefaultEula
- RequireAcceptance
- CheckinEmail

---

### 4. **OrderDto** (CheckoutRequest model) - МНОГО НЕСООТВЕТСТВИЙ!

| DTO Field Name | Model Field Name | Status |
|---------------|-----------------|---------|
| ~~ProductId~~ | **RequestableId** | ❌ WRONG |
| ~~ProductType~~ | **RequestableType** | ❌ WRONG |
| UserId | UserId | ✅ OK |
| Quantity | Quantity | ✅ OK |
| ~~RequestedAt~~ | **CreatedAt** | ❌ WRONG |
| CanceledAt | CanceledAt | ✅ OK |
| FulfilledAt | FulfilledAt | ✅ OK |
| - | UpdatedAt | ❌ MISSING |

**ОТСУТСТВУЮТ в модели:**
- Price, TotalPrice (НЕТ в CheckoutRequest!)
- Status (вычисляется из дат)
- ProductName, UserName (связи)
- Notes, DeclineReason (НЕТ в модели)

---

### 5. **CheckoutAcceptanceDto** - НЕСООТВЕТСТВИЯ

| DTO Field Name | Model Field Name | Status |
|---------------|-----------------|---------|
| ~~OrderId~~ | **CheckoutableId** | ❌ WRONG |
| ~~AssignedToUserId~~ | **AssignedToId** | ❌ WRONG |
| ~~SignatureFile~~ | **SignatureFilename** | ❌ WRONG |
| ~~EulaAccepted~~ | **StoredEula/StoredEulaFile** | ❌ WRONG |
| Note | Note | ✅ OK |
| AcceptedAt | AcceptedAt | ✅ OK |
| DeclinedAt | DeclinedAt | ✅ OK |

**ОТСУТСТВУЮТ в DTO:**
- CheckoutableType (тип объекта)
- StoredEula, StoredEulaFile
- CreatedAt, UpdatedAt

---

### 6. **AccessoryDto** - В основном OK

| DTO Field Name | Model Field Name | Status |
|---------------|-----------------|---------|
| ~~Quantity~~ | **Qty** | ❌ WRONG |
| ~~MinAmount~~ | **MinAmt** | ❌ WRONG |
| Name | Name | ✅ OK |
| CategoryId | CategoryId | ✅ OK |
| LocationId | LocationId | ✅ OK |
| PurchaseDate | PurchaseDate | ✅ OK |
| PurchaseCost | PurchaseCost | ✅ OK |
| OrderNumber | OrderNumber | ✅ OK |
| CompanyId | CompanyId | ✅ OK |
| ManufacturerId | ManufacturerId | ✅ OK |
| ModelNumber | ModelNumber | ✅ OK |
| Image | Image | ✅ OK |
| SupplierId | SupplierId | ✅ OK |
| Notes | Notes | ✅ OK |
| Requestable | Requestable | ✅ OK |
| CreatedAt | CreatedAt | ✅ OK |
| UpdatedAt | UpdatedAt | ✅ OK |

---

### 7. **StatusLabelDto** - OK

Все поля совпадают! ✅

---

### 8. **ManufacturerDto** - OK

Все поля совпадают! ✅

---

### 9. **ModelDto** - НЕСООТВЕТСТВИЯ

| DTO Field Name | Model Field Name | Status |
|---------------|-----------------|---------|
| ~~MinAmount~~ | **MinAmt** | ❌ WRONG |
| ~~EndOfLife~~ | **Eol** | ❌ WRONG |
| Name | Name | ✅ OK |
| ModelNumber | ModelNumber | ✅ OK |
| ManufacturerId | ManufacturerId | ✅ OK |
| CategoryId | CategoryId | ✅ OK |
| DepreciationId | DepreciationId | ✅ OK |
| Image | Image | ✅ OK |
| Notes | Notes | ✅ OK |
| Requestable | Requestable | ✅ OK |
| CreatedAt | CreatedAt | ✅ OK |
| UpdatedAt | UpdatedAt | ✅ OK |

**ОТСУТСТВУЮТ в DTO:**
- FieldsetId (custom fields)
- DeprecatedMacAddress

---

### 10. **LocationDto** - OK

Все поля совпадают! ✅

---

### 11. **SupplierDto** - OK

Все поля совпадают! ✅

---

## 📋 SUMMARY

### DTOs требующие исправления:

1. **UserDto** - 6 неправильных названий полей
2. **ProductDto** - отсутствует ModelId, неправильные названия
3. **CategoryDto** - Type → CategoryType
4. **OrderDto** - ProductId → RequestableId, ProductType → RequestableType
5. **CheckoutAcceptanceDto** - 4 неправильных названия
6. **AccessoryDto** - Quantity → Qty, MinAmount → MinAmt
7. **ModelDto** - MinAmount → MinAmt, EndOfLife → Eol

### DTOs которые OK:
- ✅ StatusLabelDto
- ✅ ManufacturerDto
- ✅ LocationDto
- ✅ SupplierDto
- ✅ LoginDto (не связан с моделью)
- ✅ RegisterDto (не связан с моделью)

---

## ⚠️ ВАЖНО!

**DTO поля НЕ ОБЯЗАНЫ полностью совпадать с моделью!**

DTOs могут:
1. **Иметь вычисляемые поля** (FullName, Initials)
2. **Включать связанные данные** (CategoryName вместо только CategoryId)
3. **Упрощать названия** для удобства frontend
4. **Скрывать технические поля** (Password, RememberToken)

**НО** если поле DTO должно мапиться на поле модели, названия ДОЛЖНЫ совпадать для AutoMapper!

