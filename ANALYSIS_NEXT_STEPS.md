# 📖 Объяснение для чайника + план действий

> Дата: 2 марта 2026  
> Для: DeusWork + напарник  

---

## 🤔 Сначала: что вообще происходит?

### Что такое Snipe-IT?

Представь себе **Excel-таблицу в виде сайта** для учёта школьного оборудования.  
Учителя/сисадмины заходят на `http://192.168.115.187:8050` и видят:
- Список всех ноутбуков, мониторов, кабелей
- Кому что выдано
- Когда куплено и сколько стоит

Вот и всё. Snipe-IT — это инструмент учёта, как инвентаризация на складе.  
**Он не имеет никакого отношения к интернет-магазину.** Это просто источник данных — список товаров.

---

### Что такое база данных (MariaDB)?

Snipe-IT хранит все свои данные в базе данных — **MariaDB**.  
Это как большая папка с Excel-файлами, только быстрее и надёжнее.

Внутри этой базы есть таблицы:
```
assets              ← все ноутбуки, мониторы и т.д. (это твои "товары")
users               ← все пользователи Snipe-IT
categories          ← категории товаров (Laptops, Monitors...)
manufacturers       ← производители (Dell, HP, Apple...)
accessories         ← аксессуары (кабели, мышки...)
```

**Твой .NET проект читает данные из этих таблиц** и показывает их как интернет-магазин.

---

### Как это всё связано?

```
┌─────────────────────────────────────────────────────┐
│           Учебный сервер 192.168.115.187             │
│                                                      │
│  ┌─────────────────┐    ┌──────────────────────┐    │
│  │   Snipe-IT      │    │   MariaDB (база)      │    │
│  │   (сайт)        │◄──►│                      │    │
│  │ :8050           │    │  assets               │    │
│  └─────────────────┘    │  users                │    │
│                         │  categories           │    │
│  ┌─────────────────┐    │  manufacturers        │    │
│  │  Твой магазин   │    │  ...                  │    │
│  │  (.NET проект)  │◄──►│                      │    │
│  │  :8060          │    │  [твои новые таблицы] │    │
│  └─────────────────┘    └──────────────────────┘    │
└─────────────────────────────────────────────────────┘
```

**Оба приложения** — Snipe-IT и твой магазин — работают с **одной и той же базой данных**.  
Snipe-IT читает свои таблицы. Твой магазин читает те же таблицы + добавляет свои.

---

### Почему нужны новые таблицы?

Твой магазин — это не просто каталог. У него есть функции, которых **нет в Snipe-IT**:

| Функция магазина | Где хранить данные? | Таблица |
|-----------------|---------------------|---------|
| Пользователь вошёл в магазин (JWT сессия) | Нигде в Snipe-IT нет | `webshop_refresh_tokens` |
| У пользователя есть 500 кредитов | Нигде в Snipe-IT нет | `webshop_user_credits` |
| Пользователь купил ноутбук за 200 кредитов | Нигде в Snipe-IT нет | `webshop_credit_transactions` |

**Snipe-IT не знает про кредиты.** Это наша фича. Значит, нам нужны свои таблицы.

---

### Почему не PostgreSQL, а MariaDB?

Потому что **сервер уже есть** — MariaDB на `192.168.115.187`.  
Поднимать ещё один сервер PostgreSQL ради 3 таблиц — это лишняя работа и лишний контейнер.

**Наши таблицы называются `webshop_*`** — чтобы не перепутать со Snipe-IT таблицами.  
Snipe-IT их не видит, не трогает, не знает об их существовании.

---

## ✅ ЛУЧШИЙ ВАРИАНТ: создать 3 таблицы в той же MariaDB

**Почему это лучше всего:**
- Сервер уже запущен — ничего не надо поднимать
- Данные в одном месте — проще бекапить
- Prefix `webshop_` гарантирует что не сломаем Snipe-IT
- Таблицы живут в Docker volume `db_data` — переживут перезапуск

---

## 🪜 ШАГ ЗА ШАГОМ: как создать таблицы

### Что происходит технически:

```
Твой Mac  ──SSH туннель──►  Сервер 192.168.115.187
                                        │
                                        ▼
                              Docker контейнер snipeit-db-1
                                        │
                                        ▼
                                MariaDB база snipeit
                                        │
                                        ▼
                              Создаём 3 новые таблицы
```

SSH туннель — это как **зашифрованный провод** между твоим Mac и сервером.  
Порт `3307` на твоём Mac = порт `3306` базы данных на сервере.

---

### Вариант A: Через TablePlus (GUI, самый простой)

**Шаг 1: Скачай TablePlus**

Скачай бесплатно: [https://tableplus.com/](https://tableplus.com/)  
Это как "Excel для баз данных" — красивый интерфейс, кликать мышкой.

---

**Шаг 2: Открой SSH туннель**

Открой Терминал (Terminal.app или iTerm) и введи:
```bash
ssh -L 3307:127.0.0.1:3306 -N root@192.168.115.187
```
Введи пароль: `Merc2024!`

Терминал "завис" и ничего не печатает — **это нормально**.  
Значит туннель работает. Не закрывай это окно.

```
Что происходит:
Твой Mac:3307  ═══════════════►  Сервер:3306 (база данных)
               SSH туннель
```

---

**Шаг 3: Подключись в TablePlus**

Открой TablePlus → кнопка `+` (New Connection) → выбери `MySQL`:

```
Host:     127.0.0.1
Port:     3307
User:     snipeit
Password: Merc2024!
Database: snipeit
```

Нажми `Test` — должно быть зелёное `Connected`.  
Нажми `Connect`.

---

**Шаг 4: Запусти SQL миграцию**

В TablePlus нажми `SQL` (или Ctrl+Shift+Q) — откроется окно для SQL запросов.

Нажми `File → Open` и выбери файл:
```
/Users/deuswork/Documents/Programmering/WebShopMercantec/migrations/001_webshop_tables.sql
```

Нажми кнопку `Run` (▶) или `Cmd+Enter`.

В нижней части увидишь:
```
Migration 001_webshop_tables completed successfully
```

---

**Шаг 5: Проверь что таблицы появились**

В TablePlus слева в списке таблиц должны появиться:
```
webshop_credit_transactions
webshop_refresh_tokens
webshop_user_credits
```

Если видишь — **готово**. Таблицы созданы.

---

### Вариант B: Через терминал (если нет TablePlus)

**Шаг 1: Проверь есть ли mysql на Mac**
```bash
mysql --version
```

Если видишь `mysql  Ver 8.x...` — отлично, переходи к шагу 3.  
Если видишь `command not found` — выполни шаг 2.

---

**Шаг 2: Установи mysql клиент**
```bash
brew install mysql-client
echo 'export PATH="/opt/homebrew/opt/mysql-client/bin:$PATH"' >> ~/.zshrc
source ~/.zshrc
```

---

**Шаг 3: Открой SSH туннель (первый терминал)**
```bash
ssh -L 3307:127.0.0.1:3306 -N root@192.168.115.187
```
Пароль: `Merc2024!`  
Терминал завис — нормально, не закрывай.

---

**Шаг 4: Запусти миграцию (второй терминал)**

Открой НОВЫЙ терминал (Cmd+T) и введи:
```bash
mysql -h 127.0.0.1 -P 3307 -u snipeit -pMerc2024! snipeit < /Users/deuswork/Documents/Programmering/WebShopMercantec/migrations/001_webshop_tables.sql
```

Должно вывести:
```
Migration 001_webshop_tables completed successfully
```

---

**Шаг 5: Проверь**
```bash
mysql -h 127.0.0.1 -P 3307 -u snipeit -pMerc2024! snipeit -e "SHOW TABLES LIKE 'webshop%';"
```

Должно вывести:
```
+-------------------------------+
| Tables_in_snipeit (webshop%)  |
+-------------------------------+
| webshop_credit_transactions   |
| webshop_refresh_tokens        |
| webshop_user_credits          |
+-------------------------------+
```

---

### Вариант C: Прямо на сервере (если ни A ни B не работает)

```bash
# 1. Зайди на сервер
ssh root@192.168.115.187
# пароль: Merc2024!

# 2. Зайди внутрь контейнера с базой
docker exec -it snipeit-db-1 bash

# 3. Подключись к MariaDB
mysql -u snipeit -pMerc2024! snipeit

# 4. Теперь ты внутри базы. Скопируй и вставь этот SQL:
```

```sql
CREATE TABLE IF NOT EXISTS webshop_user_credits (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT UNSIGNED NOT NULL,
    available_credits DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    total_spent DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UNIQUE KEY uq_user_id (user_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS webshop_credit_transactions (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT UNSIGNED NOT NULL,
    amount DECIMAL(10,2) NOT NULL,
    type ENUM('credit','debit','refund') NOT NULL,
    reason VARCHAR(500),
    balance_before DECIMAL(10,2) NOT NULL,
    balance_after DECIMAL(10,2) NOT NULL,
    related_order_id INT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_user_id (user_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS webshop_refresh_tokens (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT UNSIGNED NOT NULL,
    token VARCHAR(500) NOT NULL,
    expires_at DATETIME NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    revoked_at DATETIME NULL,
    replaced_by_token VARCHAR(500) NULL,
    INDEX idx_token (token(100)),
    INDEX idx_user_id (user_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

SELECT 'Migration 001_webshop_tables completed successfully' AS status;
```

```bash
# 5. Выйди из mysql и контейнера
exit
exit
```

---

## 🧪 ПРОВЕРИТЬ ЧТО ВСЁ РАБОТАЕТ

### Тест 1: Каталог (работает уже сейчас, без таблиц)

```bash
# Терминал 1: туннель
ssh -L 3307:127.0.0.1:3306 -N root@192.168.115.187

# Терминал 2: запусти проект
cd /Users/deuswork/Documents/Programmering/WebShopMercantec/WebShopMercantec/WebShopMercantec
dotnet run
```

Открой в браузере: `http://localhost:5000/swagger`

Нажми на `GET /api/products` → `Try it out` → `Execute`  
Должен вернуться список товаров из Snipe-IT с реальными данными.

```
GET /api/products           → список товаров
GET /api/categories         → категории (Laptops, Monitors...)
GET /api/manufacturers      → производители (Dell, HP...)
GET /health                 → должно быть {"status":"Healthy"}
```

---

### Тест 2: Логин (после создания таблиц)

В Swagger:  
`POST /api/auth/login` → `Try it out` → вставь:
```json
{
  "username": "superadmin",
  "password": "Merc2024!"
}
```

Должно вернуть:
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "abc123...",
  "expiresAt": "2026-03-02T...",
  "user": {
    "username": "superadmin",
    "role": "Admin"
  }
}
```

Этот `accessToken` — твой пропуск. Скопируй его, нажми кнопку `Authorize` (🔒) вверху Swagger, вставь туда `Bearer <токен>`.

---

### Тест 3: Начислить кредиты и создать заказ (после Auth)

```
POST /api/credits/users/1/add
  Body: { "amount": 1000, "reason": "Welcome bonus" }

POST /api/orders
  Body: { "requestableId": 1, "requestableType": "asset", "quantity": 1 }

POST /api/orders/1/approve   → одобрить заказ
```

---

## 📊 ЧТО ЕСТЬ В ПРОЕКТЕ ПРЯМО СЕЙЧАС

### Уже работает (0 ошибок компиляции):

| Что | Где |
|-----|-----|
| API каталога товаров и аксессуаров | `ProductsController` |
| Пагинация, поиск, фильтры по категории | `GET /api/products/paged` |
| Реальные данные (не NULL/Unknown) | `ProductMapping.MapFromDetails` |
| Категории с кол-вом товаров | `CategoriesController` |
| Производители, поставщики, локации, статусы | 4 контроллера |
| Логи в файл + консоль | `logs/webshop-*.txt` |
| Обработка ошибок (красивые JSON ответы) | `ErrorHandlingMiddleware` |
| Проверка здоровья `/health` | `HealthChecks` |
| Docker (Dockerfile + docker-compose) | Готов к деплою |

### Готово в коде, нужны таблицы в БД:

| Что | Нужная таблица |
|-----|----------------|
| Логин/регистрация (JWT) | `webshop_refresh_tokens` |
| Баланс кредитов | `webshop_user_credits` |
| История покупок | `webshop_credit_transactions` |
| Заказы (create/approve/cancel) | Все три выше |
| Защита admin endpoints `[Authorize]` | JWT должен работать |

---

## ⚡ ЧТО ДЕЛАТЬ ПРЯМО СЕЙЧАС

### Порядок действий:

```
1. Открой SSH туннель ────────────────────────────────────► 2 мин
2. Запусти dotnet run ────────────────────────────────────► 1 мин
3. Проверь каталог в Swagger ─────────────────────────────► 5 мин
4. Создай таблицы (TablePlus или терминал) ───────────────► 5 мин
5. Протестируй логин через Swagger ───────────────────────► 2 мин
6. Начните делать фронтенд (Blazor страницы) ─────────────► следующий этап
```

### Если что-то пошло не так:

| Проблема | Причина | Решение |
|----------|---------|---------|
| `dotnet run` падает с "ConnectionString is empty" | Нет `appsettings.Development.json` | Файл должен быть, он в `.gitignore` — создай вручную |
| `dotnet run` падает с "Jwt:Key is empty" | Нет ключа в appsettings | Добавь любую строку 32+ символа в `"Key"` |
| SSH туннель не подключается | Нет доступа к серверу | Проверь что ты в школьной сети / VPN |
| TablePlus "Connection failed" | Туннель не открыт | Убедись что Терминал 1 с SSH открыт и завис |
| `/api/products` возвращает `[]` | В Snipe-IT нет данных | Зайди в Snipe-IT, проверь есть ли assets |

---

## 🗂️ СТРУКТУРА ПРОЕКТА (что где лежит)

```
WebShopMercantec/
│
├── migrations/
│   └── 001_webshop_tables.sql     ← SQL для создания наших таблиц
│
├── docker-compose.yml              ← Запуск в Docker (для деплою)
├── Dockerfile                      ← Сборка образа
├── .env.example                    ← Шаблон для паролей (скопируй в .env)
│
└── WebShopMercantec/
    └── WebShopMercantec/           ← Основной проект (.NET)
        │
        ├── Controllers/            ← Endpoint-ы (что отвечает на запросы)
        │   ├── ProductsController  ← GET /api/products (каталог)
        │   ├── AuthController      ← POST /api/auth/login (логин)
        │   ├── OrdersController    ← POST /api/orders (заказы)
        │   ├── UsersController     ← GET /api/users/me (профиль)
        │   └── CreditsController   ← POST /api/credits (кредиты)
        │
        ├── Services/               ← Бизнес-логика
        │   ├── ProductService      ← Работа с товарами
        │   ├── AuthService         ← Логин, регистрация, JWT
        │   ├── CreditService       ← Начисление/списание кредитов
        │   └── OrderService        ← Создание и обработка заказов
        │
        ├── Models/                 ← C# классы = таблицы БД
        │   ├── SnipeItContext.cs   ← Scaffolded (НЕ ТРОГАЕМ!)
        │   ├── Asset.cs            ← Товар (ноутбук/монитор)
        │   ├── User.cs             ← Пользователь
        │   ├── WebShopUserCredits  ← [НАША] Кредиты
        │   └── RefreshToken        ← [НАША] JWT сессии
        │
        ├── Repositories/           ← Работа с БД (запросы)
        └── Mapping/                ← Конвертация модели → JSON ответ
```
