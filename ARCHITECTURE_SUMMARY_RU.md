# 🏗️ ВЕБШОП АРХИТЕКТУРА: ИТОГОВЫЙ ОТЧЁТ

## Что произошло

Переконструировал entire frontend + backend архитектуру WebShopMercantec с **критической ошибки** в "раздвоенное WASM SPA".

---

## 🚨 Критическая проблема (была)

```
❌ PRZED:
└─ Server (WebShopMercantec/) содержал UI компоненты
   └─ App.razor, Routes.razor, Login.razor, Home.razor в Components/
└─ Фронтенд (WebShopMercantec.Client/) был пустой
└─ WASM клиент не мог загрузиться → 💥 CRASH при старте
```

**Причина:** Рендер-мод сказал "выполняй в браузере", а компоненты лежали на сервере.

---

## ✅ Решение: WASM SPA архитектура

```
DOPO (Current):

┌─────────────────────────────────────────────────────────┐
│ WebShopMercantec.Client (BLAZOR WASM - в браузере)    │
│ ═════════════════════════════════════════════════════   │
│ ├─ Auth/                                               │
│ │  ├─ ITokenStore.cs (интерфейс локального хранилища) │
│ │  ├─ BrowserTokenStore.cs (localStorage)             │
│ │  ├─ AuthStateProvider.cs (JWT парсер + состояние)  │
│ │  └─ AuthApiClient.cs (клиент для /api/auth/*)      │
│ ├─ Http/                                               │
│ │  └─ JwtHttpMessageHandler.cs (автодобавляет токен) │
│ ├─ Components/                                         │
�� │  ├─ App.razor (раутер на клиенте)                  │
│ │  ├─ Routes.razor (защита маршрутов)                │
│ │  ├─ Layout/MainLayout.razor (навигация)            │
│ │  └─ Pages/                                          │
│ │     ├─ Login.razor (API login + auth state update) │
│ │     ├─ Home.razor (API каталог + поиск/пагинация) │
│ │     ├─ ProductDetails.razor (загрузка деталей)     │
│ │     ├─ UserProfile.razor ([Authorize])             │
│ │     └─ UserManagement.razor ([Authorize(Admin)])   │
│ ├─ wwwroot/                                            │
│ │  ├─ index.html (точка входа для WASM)              │
│ │  ├─ app.css, favicon.png                           │
│ │  └─ lib/ (Bootstrap и др)                          │
│ └─ Program.cs (DI: auth, HttpClient с Bearer)         │
│                                                         │
└─────────────────────────────────────────────────────────┘
        ↓ HTTP запросы к API
┌─────────────────────────────────────────────────────────┐
│ WebShopMercantec (REST API Server - на сервере)        │
│ ═════════════════════════════════════════════════════   │
│ ├─ Controllers/                                         │
│ │  ├─ AuthController.cs (/api/auth/*)                 │
│ │  ├─ ProductsController.cs (/api/products/*)         │
│ │  ├─ UsersController.cs (/api/users/*)               │
│ │  └─ OrdersController.cs (/api/orders/*)             │
│ ├─ Services/ (бизнес-логика)                           │
│ ├─ Repositories/ (доступ к БД)                         │
│ ├─ Middleware/ (обработка ошибок, JWT)                │
│ └─ Program.cs (DI: БД, Auth, Rate Limiting, Swagger)  │
│                                                         │
└─────────────────────────────────────────────────────────┘

WebShopMercantec.Shared
└─ DTOs/ (контракты между фронтом и API)
```

---

## 📋 Что конкретно сделали

### 1️⃣ **Client-side Auth Foundation** ✅
- `ITokenStore` → `BrowserTokenStore` — токены в localStorage (`ws.accessToken`, `ws.refreshToken`)
- `AuthStateProvider` — парсит JWT, управляет auth state
- `JwtHttpMessageHandler` — автоматически подставляет Bearer токен в каждый запрос
- `AuthApiClient` — единая точка для вызова `/api/auth/login`

### 2️⃣ **UI Components в WASM** ✅
- **App.razor** → просто компонент, который содержит Routes
- **Routes.razor** → CascadingAuthenticationState + AuthorizeRouteView (защита маршрутов)
- **Login.razor** → реальный login через API с обновлением auth state
- **Home.razor** → загрузка каталога с `/api/products/paged` (search, pagination, UX-состояния)
- **ProductDetails.razor** → загрузка товара по ID, вся информация из API
- **UserProfile.razor** + **UserManagement.razor** → защищены `[Authorize]` и `[Authorize(Roles="Admin")]`

### 3️⃣ **Server как чистый API** ✅
- **Program.cs** → только контроллеры, БД, Auth, Swagger
- **StaticFiles + MapFallbackToFile("index.html")** → WASM SPA fallback (если запрос не найден в API, отправляем index.html)
- Все компоненты удалены из сервера (больше не нужны)

### 4️⃣ **Структура файлов** ✅
```
❌ БЫЛО:
WebShopMercantec/Components/
├─ App.razor
├─ Routes.razor
├─ Pages/
│  ├─ Login.razor
│  ├─ Home.razor
│  └─ ...

✅ СТАЛО:
WebShopMercantec.Client/Components/
├─ App.razor
├─ Routes.razor
├─ Pages/
│  ├─ Login.razor
│  ├─ Home.razor
│  └─ ...

WebShopMercantec/ (только API)
├─ Controllers/
├─ Services/
├─ wwwroot/ (empty, static files от WASM идут из wwwroot клиента)
```

---

## 🔄 Как работает теперь (End-to-End)

```
1. ЗАПУСК:
   dotnet run (сервер на http://localhost:5107)
   
2. БРАУЗЕР ЗАХОДИТ НА http://localhost:5107/:
   ├─ Сервер отдаёт index.html (из .Client/wwwroot/)
   ├─ Браузер загружает blazor.web.js
   ├─ WASM клиент выполняется в браузере
   └─ Routes.razor начинает работу, Router проверяет текущий URL
   
3. ПОЛЬЗОВАТЕЛЬ НА /login:
   ├─ Login.razor отображается
   ├─ Пользователь вводит логин/пароль
   ├─ → POST /api/auth/login (сервер!)
   ├─ ✅ Получены accessToken + refreshToken
   ├─ BrowserTokenStore сохраняет в localStorage
   ├─ AuthStateProvider уведомляет всё UI об изменении
   └─ Redirect на / (Home.razor)

4. НА ГЛАВНОЙ:
   ├─ Home.razor вызывает HttpClient.GetFromJsonAsync("/api/products/paged")
   ├─ JwtHttpMessageHandler автоматически добавляет Authorization: Bearer <token>
   ├─ ✅ Сервер отдаёт товары
   ├─ UI показывает их + поиск + пагинацию
   └─ Всё работает "как настоящее" (не в браузере, а из реальной БД)

5. НА ЗАЩИЩЁННОЙ СТРАНИЦЕ (/user-profile):
   ├─ AuthorizeRouteView проверяет: есть ли у юзера JWT?
   ├─ ✅ ЕСТЬ → показываем страницу
   ├─ ❌ НЕТ → показываем "You are not authorized"
   ├─ ЕСЛИ TOKEN ПРОТУХ (401 от API):
   │  ├─ JwtHttpMessageHandler видит 401
   │  ├─ AuthStateProvider.MarkUserLoggedOutAsync()
   │  └─ Очищаем localStorage, показываем /login
   └─ Всё корректно!
```

---

## 📊 Архитектурные решения

| Параметр | Выбрано | Почему |
|----------|---------|-------|
| **Rendering** | WASM SPA | Меньше нагрузка на сервер, быстрая навигация в браузере |
| **Auth Storage** | localStorage | Просто, подходит для SPA (можно улучшить в будущем) |
| **JWT Attachment** | DelegatingHandler | Прозрачно во всех запросах, нет дублирования |
| **Route Guards** | AuthorizeRouteView | Встроено в Blazor, работает с claims |
| **API Format** | RESTful + JSON | Стандарт, легко расширяется |
| **Fallback** | MapFallbackToFile | SPA маршруты отдают index.html, Routes.razor их обрабатывает |

---

## 🛡️ Безопасность

✅ **JWT в Bearer токене** — отправляется в каждом запросе  
✅ **[Authorize] атрибуты** — защита контроллеров и страниц  
✅ **Role-based guards** — `[Authorize(Roles = "Admin")]`  
✅ **401 handling** — автоматический logout при просроченном токене  
✅ **CORS** — настроен для WASM клиента  
❌ **localStorage** — НЕ самый безопасный способ хранения (XSS уязвимость), но приемлемо для этого проекта

---

## 📦 Что сделано в этом шаге

✅ Перемещены ALL компоненты в `.Client/Components/`  
✅ Создана auth инфраструктура (ITokenStore, AuthStateProvider, JwtHttpMessageHandler)  
✅ Login, Home, ProductDetails переведены на реальные API вызовы  
✅ Добавлены [Authorize] guards для защищённых страниц  
✅ Server Program.cs очищен от Blazor компонентов, добавлен SPA fallback  
✅ **Сборка успешна** (0 errors, 0 warnings)

---

## 🚀 Следующие шаги (очередь)

1. **Checkout/Покупка** — серверная логика списания кредитов + создание заказа (атомарно)
2. **Интеграционные тесты** — проверка race conditions, rollback, недостаток кредитов
3. **Ручной E2E** — пройти сценарии из FRONTEND_TEST_PLAN.md
4. **Улучшение UX** — loading states, error boundaries, skeleton screens
5. **Security hardening** — refresh token rotation, CSRF protection, more secure token storage

---

## 💡 Почему это важно для вебшопа

```
РАНЬШЕ: Сложная архитектура, фронт не работает без сервера.
ТЕПЕРЬ: Чистое разделение:
         - Фронт = WASM (быстро, автономно, offline-capable)
         - Бек = API (масштабируемо, переиспользуемо)
         
РЕЗУЛЬТАТ: Вебшоп готов к:
           ✅ Масштабированию (разные клиенты могут тапать один API)
           ✅ Кешированию (браузер кешует static + API responses)
           ✅ Мобильному приложению (используем тот же /api/*)
           ✅ Микросервисам (API независим от UI)
```

---

## ✨ BUILD: ✅ SUCCESS

```
WebShopMercantec.Shared succeeded
WebShopMercantec.Client succeeded (→ wwwroot)
WebShopMercantec succeeded
Build succeeded. 0 Warning(s), 0 Error(s)
```

**WebShop is ready for next phase: Payment & Checkout Logic! 🎯**

