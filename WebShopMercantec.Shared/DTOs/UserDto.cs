namespace WebShopMercantec.Shared.DTOs;

public class UserDto
{
    // Основные поля
    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    // Имя пользователя
    public string FirstName { get; set; } = string.Empty;
    
    public string LastName { get; set; } = string.Empty;
    
    // Полное имя (вычисляемое)
    public string FullName => $"{FirstName} {LastName}".Trim();

    // Аватар/фото профиля
    public string? Avatar { get; set; }
    
    // Инициалы для отображения (JD для John Doe)
    public string Initials => $"{FirstName.FirstOrDefault()}{LastName.FirstOrDefault()}".ToUpper();

    // Роль/права доступа
    public string Role { get; set; } = "User"; // User, Admin, Manager
    
    public string? Permissions { get; set; }

    // КРЕДИТЫ - основная валюта в системе
    public decimal AvailableCredits { get; set; }
    
    public decimal TotalCreditsSpent { get; set; }

    // Статистика пользователя
    public int TotalPurchases { get; set; }

    // Контактная информация
    public string? Phone { get; set; }
    
    public string? JobTitle { get; set; }

    // Местоположение и организация
    public int? LocationId { get; set; }
    
    public string? LocationName { get; set; }
    
    public int? DepartmentId { get; set; }
    
    public string? DepartmentName { get; set; }
    
    public int? CompanyId { get; set; }
    
    public string? CompanyName { get; set; }

    // Адрес
    public string? Address { get; set; }
    
    public string? City { get; set; }
    
    public string? State { get; set; }
    
    public string? Country { get; set; }
    
    public string? Zip { get; set; }

    // Статус активации
    public bool Activated { get; set; }
    
    public bool ShowInList { get; set; } = true;

    // Менеджер
    public int? ManagerId { get; set; }
    
    public string? ManagerName { get; set; }

    // Номер сотрудника
    public string? EmployeeNumber { get; set; }

    // Даты работы
    public DateTime? StartDate { get; set; }
    
    public DateTime? EndDate { get; set; }

    // Дата регистрации (Member Since)
    public DateTime? CreatedAt { get; set; }
    
    public DateTime? LastLogin { get; set; }

    // VIP статус (для особых пользователей)
    public bool IsVip { get; set; }
    
    // Удаленная работа
    public bool IsRemote { get; set; }

    // Дополнительная информация
    public string? Notes { get; set; }
    
    public string? Website { get; set; }
}