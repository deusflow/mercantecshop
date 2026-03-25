# IMPLEMENTATION_NEXT_STEPS_RU.md

## Цель
Закрыть фронтенд-критичные пробелы и довести backend checkout до production-ready состояния, включая задачу: **"дописать логику корзины и покупок на сервере (списание кредитов + оформление заказа)"**.

## Что уже реализовано в этом шаге
- Внедрен client auth foundation: `ITokenStore`, `BrowserTokenStore`, `AuthStateProvider`, `JwtHttpMessageHandler`, `AuthApiClient`.
- Настроен `HttpClient` с Bearer JWT в `WebShopMercantec/WebShopMercantec/WebShopMercantec.Client/Program.cs`.
- Включен auth-aware routing в `WebShopMercantec/WebShopMercantec/WebShopMercantec/Components/Routes.razor`.
- Исправлен layout navigation (`NavLink`, удален мусорный символ) в `WebShopMercantec/WebShopMercantec/WebShopMercantec/Components/Layout/MainLayout.razor`.
- `Login.razor` переведен на реальный API login + обновление auth state.
- `Home.razor` переведен на API каталог (`/api/products/paged`) с search/pagination/loading/error/empty.
- `ProductDetails.razor` загружает товар по `id` с API (`/api/products/{id}`).
- Добавлены route guards:
  - `WebShopMercantec/WebShopMercantec/WebShopMercantec/Components/Pages/UserProfile.razor` -> `[Authorize]`
  - `WebShopMercantec/WebShopMercantec/WebShopMercantec/Components/Pages/UserManagement.razor` -> `[Authorize(Roles = "Admin")]`

## Что сделать самостоятельно (детальный чеклист)

### Итерация 1 — стабилизация auth и UX (P0)
- [ ] Прогнать `FRONTEND_TEST_PLAN.md` полностью.
- [ ] Проверить токены в `localStorage` (`ws.accessToken`, `ws.refreshToken`) после логина.
- [ ] Убедиться, что `401` с API переводит UI в разлогиненное состояние.
- [ ] Проверить role-доступ для `/admin/users` на реальном admin JWT.
- [ ] DoD: нет blank-page, нет full reload, все protected pages отрабатывают предсказуемо.

Как проверять:
1. Browser DevTools -> Network + Application/Storage.
2. Сценарий: login -> refresh page -> удаление токена -> переход на protected route.
3. Ожидание: UI сообщает об отсутствии доступа, без зависаний.

### Итерация 2 — доведение каталога/деталей (P0)
- [ ] Проверить, что API пагинации действительно возвращает разные страницы.
- [ ] Проверить поведение поиска на пустой строке и спецсимволах.
- [ ] Добавить fallback UX для `404` в `ProductDetails.razor` (текст уже есть, проверить визуально).
- [ ] Добавить smoke-проверку: нет дублирующихся запросов при быстрых переходах.
- [ ] DoD: каталог и детали работают без статических заглушек.

Где смотреть:
- `WebShopMercantec/WebShopMercantec/WebShopMercantec/Components/Pages/Home.razor`
- `WebShopMercantec/WebShopMercantec/WebShopMercantec/Components/Pages/ProductDetails.razor`
- `WebShopMercantec/WebShopMercantec/WebShopMercantec/Controllers/ProductsController.cs`

### Итерация 3 — backend: корзина и покупки (P0)

#### 3.1 Контракт API checkout
- [ ] Определить endpoint checkout (например `POST /api/orders/checkout`) и payload (items, quantity, notes).
- [ ] Для текущего минимального варианта принять `OrderCreateDto` и оформить один товар за запрос.
- [ ] Зафиксировать ответы: `200/201`, `400`, `401`, `402` (insufficient credits), `409` (not available).

Где:
- `WebShopMercantec/WebShopMercantec/WebShopMercantec/Controllers/OrdersController.cs`
- `WebShopMercantec/WebShopMercantec.Shared/DTOs/AuthResponseDto.cs` (где уже есть `OrderCreateDto`)

#### 3.2 Сервисная оркестрация покупки
- [ ] В `OrderService` сделать единый метод `PurchaseAsync(userId, dto)`.
- [ ] Внутри: валидация запроса -> проверка доступности -> расчёт цены -> списание кредитов -> создание заказа -> лог транзакции.
- [ ] Возвращать детальный результат для UI (id заказа, списанные кредиты, остаток).

Где:
- `WebShopMercantec/WebShopMercantec/WebShopMercantec/Services/OrderService.cs`
- `WebShopMercantec/WebShopMercantec/WebShopMercantec/Services/CreditService.cs`

#### 3.3 Атомарность и конкурентный доступ
- [ ] Обернуть списание+создание заказа в единую БД-транзакцию.
- [ ] Добавить защиту от double-submit (идемпотентность или уникальный ключ запроса).
- [ ] Проверить параллельные покупки одним пользователем: баланс не уходит в минус.

Где:
- `WebShopMercantec/WebShopMercantec/WebShopMercantec/Repositories/UnitOfWork.cs`
- `WebShopMercantec/WebShopMercantec/WebShopMercantec/Repositories/Specific/OrderRepository.cs`
- `migrations/` (при необходимости новых индексов/ограничений)

#### 3.4 Валидации и ошибки
- [ ] Добавить/обновить валидатор для `OrderCreateDto` (quantity > 0, допустимый type).
- [ ] Вернуть из middleware единый формат ошибок для фронта.
- [ ] Добавить бизнес-исключения: недостаточно кредитов, недоступный товар, конфликт количества.

Где:
- `WebShopMercantec/WebShopMercantec/WebShopMercantec/Validators/`
- `WebShopMercantec/WebShopMercantec/WebShopMercantec/Middleware/`
- `WebShopMercantec/WebShopMercantec/WebShopMercantec/Exceptions/`

#### 3.5 Аудит и наблюдаемость
- [ ] Логировать `userId`, `requestableId`, `amount`, `result`, `orderId`.
- [ ] Добавить бизнес-лог события "purchase completed/failed".

Где:
- `WebShopMercantec/WebShopMercantec/WebShopMercantec/Services/OrderService.cs`
- `WebShopMercantec/WebShopMercantec/WebShopMercantec/Models/ActionLog.cs` (если используется)

### Итерация 4 — тесты (P0)
- [ ] Добавить интеграционные тесты checkout (happy path, insufficient credits, rollback, unavailable item).
- [ ] Добавить ручной E2E прогон: login -> catalog -> details -> purchase -> profile/history.
- [ ] Проверить admin-flow: просмотр/подтверждение/отклонение заказа.
- [ ] DoD: критические сценарии закрыты автоматическими или ручными тестами.

## Как проверять каждый этап

### Минимум перед merge
- [ ] `dotnet build WebShopMercantec.sln` проходит без ошибок.
- [ ] Swagger endpoint отвечает.
- [ ] В браузере нет необработанных exceptions в Console.
- [ ] По Network нет неожиданных дублирующихся API-запросов.

### Критерии готовности задачи про покупки
- [ ] Кредиты списываются ровно один раз на один успешный checkout.
- [ ] Заказ создается только если списание успешно.
- [ ] При любой ошибке внутри пайплайна транзакция откатывается.
- [ ] Возврат API прозрачен для UI и даёт user-friendly текст.

## Риски и что мониторить
- Race condition при параллельных checkout.
- Несовпадение role claim в JWT и `[Authorize(Roles="Admin")]`.
- Падение UX из-за отличающегося формата backend ошибок.
- Full reload из-за случайных `<a href>` на внутренних маршрутах.

## Шаблон для самостоятельной фиксации прогресса

### Что я сделал
- Дата:
- Ветка:
- Коммиты:
- Измененные файлы:

### Что проверил
- Сценарии:
- Результат:
- Скриншоты/заметки:

### Что осталось
- Блокеры:
- Следующий шаг:

