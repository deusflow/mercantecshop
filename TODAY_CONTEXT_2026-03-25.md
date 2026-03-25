# Today Context (2026-03-25)

## 1) Hotfix applied now (404 on localhost:5107)

### Problem observed
- `GET /_framework/blazor.web.js` -> 404
- `GET /WebShopMercantec.Client.styles.css` -> 404

### Root cause
- In `WebShopMercantec/WebShopMercantec.Client/wwwroot/index.html` the app referenced wrong asset names for current hosted WASM setup.

### Fix made
- Updated `WebShopMercantec/WebShopMercantec.Client/wwwroot/index.html`:
  - `WebShopMercantec.Client.styles.css` -> `WebShopMercantec.styles.css`
  - `_framework/blazor.web.js` -> `_framework/blazor.webassembly.js`

### Verification done
- Build: `dotnet build WebShopMercantec/WebShopMercantec/WebShopMercantec.csproj -v minimal` -> **succeeded**
- HTTP checks:
  - `GET /_framework/blazor.webassembly.js` -> **200**
  - `GET /WebShopMercantec.styles.css` -> **200**
  - `GET /` serves updated `index.html` with corrected links.

## 2) Commit(s) made today

`git log --since="2026-03-25 00:00" --name-status`

- `7391534 | 2026-03-25 12:28:54 +0100 | DeusFlow`
  - Message: `Refactor search methods in repositories to use EF.Functions.Like for improved performance; normalize search terms and enhance user input handling in UserRepository and AccessoryRepository; implement rate limiting in AuthController and Program setup; update database context configurations for clarity and consistency.`
  - Files:
    - `README.md`
    - `WebShopMercantec/WebShopMercantec/Controllers/AuthController.cs`
    - `WebShopMercantec/WebShopMercantec/Extensions/ClaimsPrincipalExtensions.cs`
    - `WebShopMercantec/WebShopMercantec/Models/SnipeItContext.WebShop.cs`
    - `WebShopMercantec/WebShopMercantec/Program.cs`
    - `WebShopMercantec/WebShopMercantec/Repositories/Specific/AccessoryRepository.cs`
    - `WebShopMercantec/WebShopMercantec/Repositories/Specific/ProductRepository.cs`
    - `WebShopMercantec/WebShopMercantec/Repositories/Specific/UserRepository.cs`
    - `WebShopMercantec/WebShopMercantec/Repositories/UnitOfWork.cs`
    - `WebShopMercantec/WebShopMercantec/Services/AuthService.cs`
    - `WebShopMercantec/WebShopMercantec/Services/CreditService.cs`
    - `WebShopMercantec/WebShopMercantec/Services/IAuthService.cs`

## 3) Current workspace change state (full transfer artifacts)

- Branch: `oleh-dev`
- Full tracked diff artifact: `TODAY_CHANGES_2026-03-25.patch`
  - Generated from: `git diff --binary HEAD`
  - Size: `3140` lines
- Untracked files list: `TODAY_UNTRACKED_2026-03-25.txt`

These two files are the recommended bundle to pass another AI:
1. `TODAY_CHANGES_2026-03-25.patch`
2. `TODAY_UNTRACKED_2026-03-25.txt`

## 4) Current untracked files

From `TODAY_UNTRACKED_2026-03-25.txt`:
- `TODAY_CHANGES_2026-03-25.patch`
- `TODAY_UNTRACKED_2026-03-25.txt`
- `WebShopMercantec/WebShopMercantec/logs/webshop-20260325.txt`

## 5) Notes

- This context file is a compact navigator.
- The patch file contains the complete line-level tracked changes relative to `HEAD` at generation time.

