namespace WebShopMercantec.Exceptions;

/// <summary>
/// Исключение, которое выбрасывается, когда запрашиваемый ресурс не найден
/// HTTP Status Code: 404 Not Found
/// 
/// КОГДА ИСПОЛЬЗОВАТЬ:
/// - Продукт с ID не существует
/// - Пользователь не найден
/// - Заказ не найден
/// 
/// ПРИМЕР:
/// var user = await _unitOfWork.Users.GetByIdAsync(userId);
/// if (user == null)
///     throw new NotFoundException($"User with ID {userId} not found");
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException() : base("Resource not found")
    {
    }

    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }

    public NotFoundException(string resourceName, object key)
        : base($"{resourceName} with key '{key}' was not found")
    {
    }
}


