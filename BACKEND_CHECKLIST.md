# ✅ Backend Development Checklist - WebShopMercantec

## 🎯 Quick Progress Tracker

---

## 📊 CURRENT STATUS: 15% Complete

### ✅ Completed (15%)
- [x] Entity Framework Core setup
- [x] MySQL connection (Pomelo)
- [x] All models created
- [x] DbContext configured
- [x] DTOs created
- [x] Swagger configured
- [x] Basic ProductsController
- [x] Basic ProductService

### 🔄 In Progress (0%)
- [ ] None currently

### ❌ Not Started (85%)
- [ ] Everything else below

---

## 🔥 PHASE 1: INFRASTRUCTURE (Critical - Week 1)

### Repository Pattern
- [x] Create `IRepository<T>` interface
- [x] Create `Repository<T>` implementation
- [x] Create `IUserRepository` + implementation
- [x] Create `IProductRepository` + implementation
- [x] Create `IOrderRepository` + implementation
- [x] Create `IAccessoryRepository` + implementation
- [x] Create `IUnitOfWork` interface
- [x] Create `UnitOfWork` implementation
- [x] Register in Program.cs

### Authentication & Authorization
- [ ] Install JWT packages (Microsoft.AspNetCore.Authentication.JwtBearer)
- [ ] Install BCrypt (BCrypt.Net-Next)
- [ ] Create JWT settings in appsettings.json
- [ ] Create `ITokenService` interface
- [ ] Create `TokenService` implementation
- [ ] Create `IAuthService` interface
- [ ] Create `AuthService` implementation
- [ ] Create `AuthController`
  - [ ] POST /api/auth/login
  - [ ] POST /api/auth/register
  - [ ] POST /api/auth/refresh
  - [ ] POST /api/auth/logout
  - [ ] GET /api/auth/me
  - [ ] POST /api/auth/change-password
- [ ] Configure JWT in Program.cs
- [ ] Add Authorization policies (AdminOnly, UserOrAdmin)
- [ ] Create refresh token mechanism
- [ ] Test authentication flow

### Error Handling & Logging
- [x] Install Serilog packages
- [x] Configure Serilog in Program.cs
- [x] Create `ErrorHandlingMiddleware`
- [x] Create custom exceptions:
  - [x] `NotFoundException`
  - [x] `UnauthorizedException`
  - [x] `BadRequestException`
  - [x] `InsufficientCreditsException`
  - [x] `ProductNotAvailableException`
  - [x] `ForbiddenException` (bonus!)
- [ ] Create `ILoggerService` for DB logging (опционально)
- [ ] Create `LoggerService` implementation (опционально)

**Estimated Time: 16-21 hours**

---

## 🚀 PHASE 2: CORE SERVICES (Critical - Week 2)

### User Management Service
- [ ] Create `IUserService` interface
- [ ] Create `UserService` implementation
  - [ ] GetUserByIdAsync
  - [ ] GetUserByUsernameAsync
  - [ ] GetUserByEmailAsync
  - [ ] GetAllUsersAsync (with pagination)
  - [ ] CreateUserAsync
  - [ ] UpdateUserAsync
  - [ ] DeleteUserAsync
  - [ ] GetUserCreditsAsync
  - [ ] AddCreditsAsync
  - [ ] DeductCreditsAsync
  - [ ] GetUserTransactionsAsync
  - [ ] GetTotalPurchasesAsync
  - [ ] GetTotalSpentAsync
  - [ ] ValidateUserCreditsAsync
  - [ ] UserExistsAsync
- [ ] Create `UsersController`
  - [ ] GET /api/users
  - [ ] GET /api/users/{id}
  - [ ] POST /api/users
  - [ ] PUT /api/users/{id}
  - [ ] DELETE /api/users/{id}
  - [ ] GET /api/users/{id}/credits
  - [ ] POST /api/users/{id}/credits/add
  - [ ] POST /api/users/{id}/credits/deduct
  - [ ] GET /api/users/{id}/transactions
  - [ ] GET /api/users/{id}/statistics
- [ ] Add [Authorize] attributes
- [ ] Test all endpoints

### Credit System
- [ ] Create `CreditTransaction` model
- [ ] Add migration for CreditTransaction table
- [ ] Add `AvailableCredits` field to User model
- [ ] Create migration for User.AvailableCredits
- [ ] Run migrations
- [ ] Create `ICreditService` interface
- [ ] Create `CreditService` implementation
  - [ ] GetBalanceAsync
  - [ ] AddCreditsAsync
  - [ ] DeductCreditsAsync
  - [ ] HasSufficientCreditsAsync
  - [ ] GetTransactionHistoryAsync
  - [ ] RefundAsync
  - [ ] GetStatisticsAsync
- [ ] Create `CreditsController` (Admin only)
  - [ ] GET /api/credits/user/{userId}
  - [ ] POST /api/credits/user/{userId}/add
  - [ ] POST /api/credits/user/{userId}/deduct
  - [ ] GET /api/credits/transactions
  - [ ] POST /api/credits/refund/{transactionId}
- [ ] Implement transaction safety (DB transactions)
- [ ] Test credit operations

### Product Service (Extended)
- [ ] Update `IProductService` interface with new methods:
  - [ ] GetProductsByCategoryAsync
  - [ ] GetProductsByManufacturerAsync
  - [ ] SearchProductsAsync
  - [ ] GetProductsPagedAsync
  - [ ] GetAvailableAccessoriesAsync
  - [ ] GetAccessoryByIdAsync
  - [ ] CreateProductAsync
  - [ ] UpdateProductAsync
  - [ ] DeleteProductAsync
  - [ ] IsProductAvailableAsync
  - [ ] GetAvailableQuantityAsync
- [ ] Update `ProductService` implementation
- [ ] Update `ProductsController`:
  - [ ] Add GET /api/products?page=1&pageSize=20&category=&search=
  - [ ] Add GET /api/products/category/{categoryId}
  - [ ] Add GET /api/products/search?q=
  - [ ] Add POST /api/products (Admin)
  - [ ] Add PUT /api/products/{id} (Admin)
  - [ ] Add DELETE /api/products/{id} (Admin)
  - [ ] Add GET /api/products/accessories
  - [ ] Add GET /api/products/accessories/{id}
- [ ] Test pagination and filtering

### Order Management Service (THE MOST IMPORTANT!)
- [ ] Create `IOrderService` interface
- [ ] Create `OrderService` implementation
  - [ ] CreateOrderAsync (with credit deduction)
  - [ ] GetOrderByIdAsync
  - [ ] GetUserOrdersAsync
  - [ ] CancelOrderAsync (with refund)
  - [ ] GetAllOrdersAsync (Admin)
  - [ ] GetPendingOrdersAsync (Admin)
  - [ ] ApproveOrderAsync (Admin)
  - [ ] DeclineOrderAsync (Admin, with refund)
  - [ ] FulfillOrderAsync (Admin)
  - [ ] GetOrderStatisticsAsync
- [ ] Create `OrdersController`
  - [ ] POST /api/orders (Create order)
  - [ ] GET /api/orders/my-orders
  - [ ] GET /api/orders/{id}
  - [ ] POST /api/orders/{id}/cancel
  - [ ] GET /api/orders (Admin - all orders)
  - [ ] GET /api/orders/pending (Admin)
  - [ ] POST /api/orders/{id}/approve (Admin)
  - [ ] POST /api/orders/{id}/decline (Admin)
  - [ ] GET /api/orders/statistics (Admin)
- [ ] Implement complete order workflow:
  - [ ] Check product availability
  - [ ] Check user credits
  - [ ] Reserve credits
  - [ ] Create checkout request
  - [ ] Handle approval (assign product to user)
  - [ ] Handle decline (refund credits)
  - [ ] Handle cancellation (refund credits)
- [ ] Add transaction safety
- [ ] Test complete order flow

### Accessory Management
- [ ] Create `IAccessoryService` interface
- [ ] Create `AccessoryService` implementation
  - [ ] GetAvailableAccessoriesAsync
  - [ ] GetAccessoryByIdAsync
  - [ ] CheckoutAccessoryAsync
  - [ ] CheckinAccessoryAsync
  - [ ] GetUserAccessoriesAsync
  - [ ] CreateAccessoryAsync (Admin)
  - [ ] UpdateAccessoryAsync (Admin)
  - [ ] DeleteAccessoryAsync (Admin)
- [ ] Create `AccessoriesController`
  - [ ] GET /api/accessories
  - [ ] GET /api/accessories/{id}
  - [ ] POST /api/accessories/checkout
  - [ ] POST /api/accessories/checkin/{checkoutId}
  - [ ] GET /api/accessories/my-accessories
  - [ ] POST /api/accessories (Admin)
  - [ ] PUT /api/accessories/{id} (Admin)
  - [ ] DELETE /api/accessories/{id} (Admin)
- [ ] Test accessory checkout/checkin

**Estimated Time: 28-36 hours**

---

## 📦 PHASE 3: SUPPORTING SERVICES (Medium - Week 3)

### Category Management
- [ ] Create `ICategoryService`
- [ ] Create `CategoryService`
- [ ] Create `CategoriesController`
  - [ ] GET /api/categories
  - [ ] GET /api/categories/{id}
  - [ ] POST /api/categories (Admin)
  - [ ] PUT /api/categories/{id} (Admin)
  - [ ] DELETE /api/categories/{id} (Admin)

### Manufacturer Management
- [ ] Create `IManufacturerService`
- [ ] Create `ManufacturerService`
- [ ] Create `ManufacturersController`
  - [ ] GET /api/manufacturers
  - [ ] GET /api/manufacturers/{id}
  - [ ] POST /api/manufacturers (Admin)
  - [ ] PUT /api/manufacturers/{id} (Admin)
  - [ ] DELETE /api/manufacturers/{id} (Admin)

### Location Management
- [ ] Create `ILocationService`
- [ ] Create `LocationService`
- [ ] Create `LocationsController`
  - [ ] GET /api/locations
  - [ ] GET /api/locations/{id}
  - [ ] POST /api/locations (Admin)
  - [ ] PUT /api/locations/{id} (Admin)
  - [ ] DELETE /api/locations/{id} (Admin)

### Supplier Management
- [ ] Create `ISupplierService`
- [ ] Create `SupplierService`
- [ ] Create `SuppliersController`
  - [ ] GET /api/suppliers
  - [ ] GET /api/suppliers/{id}
  - [ ] POST /api/suppliers (Admin)
  - [ ] PUT /api/suppliers/{id} (Admin)
  - [ ] DELETE /api/suppliers/{id} (Admin)

### Status Label Management
- [ ] Create `IStatusLabelService`
- [ ] Create `StatusLabelService`
- [ ] Create `StatusLabelsController`
  - [ ] GET /api/statuslabels
  - [ ] GET /api/statuslabels/{id}
  - [ ] POST /api/statuslabels (Admin)
  - [ ] PUT /api/statuslabels/{id} (Admin)
  - [ ] DELETE /api/statuslabels/{id} (Admin)

**Estimated Time: 10-15 hours**

---

## 🎨 PHASE 4: ADVANCED FEATURES (Medium)

### File Upload Service
- [ ] Create `IFileUploadService`
- [ ] Create `FileUploadService`
  - [ ] UploadImageAsync
  - [ ] DeleteImageAsync
  - [ ] ValidateImageAsync
  - [ ] GetImageUrl
- [ ] Add upload endpoints to controllers:
  - [ ] POST /api/products/{id}/upload-image
  - [ ] POST /api/accessories/{id}/upload-image
  - [ ] POST /api/users/{id}/upload-avatar
- [ ] Configure wwwroot/uploads folder
- [ ] Add image validation (size, format)
- [ ] Test file upload

### Email Service
- [ ] Install MailKit package
- [ ] Add EmailSettings to appsettings.json
- [ ] Create `IEmailService`
- [ ] Create `EmailService`
  - [ ] SendEmailAsync
  - [ ] SendOrderCreatedAsync
  - [ ] SendOrderApprovedAsync
  - [ ] SendOrderDeclinedAsync
  - [ ] SendCreditsAddedAsync
  - [ ] SendWelcomeEmailAsync
- [ ] Integrate with Order workflow
- [ ] Test email sending

### Search & Filtering
- [ ] Create `ISearchService`
- [ ] Create `SearchService`
- [ ] Create `SearchFilterDto`
- [ ] Create `SearchResultDto`
- [ ] Implement advanced search:
  - [ ] Full-text search
  - [ ] Filter by category
  - [ ] Filter by manufacturer
  - [ ] Filter by price range
  - [ ] Sorting options
  - [ ] Pagination
- [ ] Add endpoint: GET /api/search
- [ ] Test search functionality

### Dashboard & Statistics
- [ ] Create `IStatisticsService`
- [ ] Create `StatisticsService`
- [ ] Create `DashboardController`
  - [ ] GET /api/dashboard/statistics
  - [ ] GET /api/dashboard/recent-orders
  - [ ] GET /api/dashboard/top-products
  - [ ] GET /api/dashboard/top-users
  - [ ] GET /api/dashboard/charts/orders
  - [ ] GET /api/dashboard/charts/credits
- [ ] Create `DashboardDto`
- [ ] Implement statistics calculations
- [ ] Test dashboard endpoints

**Estimated Time: 16-20 hours**

---

## ⚡ PHASE 5: OPTIMIZATION (Medium)

### Validation
- [x] Install FluentValidation packages
- [x] Create validators:
  - [x] LoginDtoValidator
  - [x] RegisterDtoValidator
  - [x] ProductDtoValidator
  - [x] OrderDtoValidator
  - [x] UserDtoValidator
  - [x] AccessoryDtoValidator
- [x] Register validators in Program.cs
- [x] Create ValidationExtensions helper
- [x] Test validation

### Caching
- [ ] Install caching packages
- [ ] Create `ICacheService`
- [ ] Create `CacheService`
- [ ] Add caching for:
  - [ ] Categories list
  - [ ] Manufacturers list
  - [ ] Popular products
  - [ ] System settings
- [ ] Configure cache expiration
- [ ] Test cache functionality

### Rate Limiting
- [ ] Configure rate limiter in Program.cs
- [ ] Add rate limiting policies:
  - [ ] General API limit
  - [ ] Login endpoint limit
  - [ ] Register endpoint limit
  - [ ] Order creation limit
- [ ] Apply to endpoints
- [ ] Test rate limiting

### Custom Exception Handling
- [ ] Create all custom exceptions
- [ ] Update ErrorHandlingMiddleware
- [ ] Add proper error responses
- [ ] Test error handling

### API Versioning
- [ ] Install Asp.Versioning.Mvc
- [ ] Configure versioning in Program.cs
- [ ] Update controllers with [ApiVersion]
- [ ] Update routes to include version

**Estimated Time: 10-14 hours**

---

## 🔒 PHASE 6: SECURITY (Critical)

### Security Headers
- [ ] Add security headers middleware
- [ ] Configure X-Content-Type-Options
- [ ] Configure X-Frame-Options
- [ ] Configure X-XSS-Protection
- [ ] Configure Content-Security-Policy
- [ ] Test headers

### CORS Configuration
- [ ] Configure CORS policy
- [ ] Set allowed origins
- [ ] Set allowed methods
- [ ] Set allowed headers
- [ ] Enable credentials
- [ ] Test CORS

### Input Sanitization
- [ ] Create InputSanitizer utility
- [ ] Add sanitization to all controllers
- [ ] Test XSS protection
- [ ] Test SQL injection protection

### Password Policy
- [ ] Implement strong password validation
- [ ] Add password strength checker
- [ ] Enforce password requirements
- [ ] Test password policy

**Estimated Time: 5-6 hours**

---

## ✅ PHASE 7: TESTING (High Priority)

### Unit Tests
- [ ] Create test project
- [ ] Install testing packages (xUnit, Moq, FluentAssertions)
- [ ] Create test infrastructure
- [ ] Write tests for:
  - [ ] AuthService (10+ tests)
  - [ ] UserService (10+ tests)
  - [ ] ProductService (10+ tests)
  - [ ] OrderService (15+ tests)
  - [ ] CreditService (10+ tests)
  - [ ] AccessoryService (8+ tests)
- [ ] Aim for 70%+ code coverage
- [ ] Run all tests

### Integration Tests
- [ ] Create integration test project
- [ ] Setup WebApplicationFactory
- [ ] Write integration tests for:
  - [ ] AuthController (5+ tests)
  - [ ] ProductsController (8+ tests)
  - [ ] OrdersController (10+ tests)
  - [ ] UsersController (8+ tests)
- [ ] Test complete workflows
- [ ] Run all integration tests

**Estimated Time: 16-20 hours**

---

## 📚 PHASE 8: DOCUMENTATION

### Swagger Enhancement
- [ ] Update Swagger configuration
- [ ] Add JWT authentication to Swagger
- [ ] Add XML comments to all controllers
- [ ] Add XML comments to all DTOs
- [ ] Add examples to Swagger
- [ ] Test Swagger UI

### API Documentation
- [ ] Create API_DOCUMENTATION.md
- [ ] Document all endpoints
- [ ] Add request/response examples
- [ ] Document error codes
- [ ] Add authentication guide
- [ ] Add rate limiting info
- [ ] Add usage examples

### Database Seeding
- [ ] Create DbInitializer
- [ ] Create SeedData
- [ ] Add seed data:
  - [ ] Admin user (admin/admin123)
  - [ ] Test user (user/user123)
  - [ ] Categories (5-10)
  - [ ] Manufacturers (5-10)
  - [ ] Status labels (3-5)
  - [ ] Locations (3-5)
  - [ ] Test products (10-15)
  - [ ] Test accessories (5-10)
- [ ] Call DbInitializer in Program.cs
- [ ] Test seeding

### Environment Configuration
- [ ] Configure appsettings.Development.json
- [ ] Configure appsettings.Production.json
- [ ] Setup environment variables
- [ ] Document configuration

**Estimated Time: 7-10 hours**

---

## 🎯 PHASE 9: MONITORING (Optional)

### Health Checks
- [ ] Add health checks
- [ ] Add database health check
- [ ] Add API health check
- [ ] Add /health endpoint
- [ ] Test health checks

### Performance Monitoring
- [ ] Install Application Insights (optional)
- [ ] Configure monitoring
- [ ] Add custom metrics
- [ ] Test monitoring

**Estimated Time: 3-4 hours**

---

## 📊 OVERALL PROGRESS TRACKING

### Total Tasks: ~350
### Completed: ~63 (18%)

### By Phase Completion:
- [x] Phase 1.1: Repository Pattern (100% ✅)
- [ ] Phase 1.2: Authentication & Authorization (0%)
- [x] Phase 1.3: Error Handling & Logging (100% ✅)
- [x] Phase 2.3: Product Service Extended (100% ✅)
- [ ] Phase 2.1-2.2: User & Credit Services (0%)
- [ ] Phase 2.4-2.5: Order & Accessory Services (0%)
- [ ] Phase 3: Supporting Services (0%)
- [ ] Phase 4: Advanced Features (0%)
- [x] Phase 5.1: Validation (100% ✅)
- [ ] Phase 5.2-5.4: Other Optimization (0%)
- [ ] Phase 6: Security (0%)
- [ ] Phase 7: Testing (0%)
- [ ] Phase 8: Documentation (0%)
- [ ] Phase 9: Monitoring (0%)

---

## 🎯 MILESTONES

### Milestone 1: MVP (Minimum Viable Product)
**Target: End of Week 2**
- [x] Database connection
- [ ] Authentication working
- [ ] User management working
- [ ] Product listing working
- [ ] Order creation working
- [ ] Credit system working

### Milestone 2: Feature Complete
**Target: End of Week 3**
- [ ] All core services implemented
- [ ] All CRUD operations working
- [ ] Admin panel APIs ready
- [ ] All supporting services done

### Milestone 3: Production Ready
**Target: End of Week 4**
- [ ] All tests passing
- [ ] Security measures in place
- [ ] Documentation complete
- [ ] Performance optimized
- [ ] Ready to deploy

---

## 🚀 NEXT STEPS

1. **Start with Phase 1, Task 1:** Create Repository Pattern
2. **Complete one phase at a time**
3. **Test after each major feature**
4. **Commit frequently to Git**
5. **Update this checklist as you progress**

---

**Last Updated:** December 14, 2025
**Progress:** 15% Complete
**Estimated Remaining Time:** 100-130 hours

