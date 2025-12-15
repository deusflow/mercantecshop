namespace WebShopMercantec.Exceptions;

/// <summary>
/// Исключение для некорректных запросов от клиента
/// HTTP Status Code: 400 Bad Request
/// 
/// КОГДА ИСПОЛЬЗОВАТЬ:
/// - Невалидные данные (пустые поля, некорректный формат)
/// - Email уже существует
/// - Некорректные параметры запроса
/// - Нарушение бизнес-правил
/// 
/// ПРИМЕР:
/// if (await _unitOfWork.Users.EmailExistsAsync(email))
///     throw new BadRequestException("Email already in use");
/// </summary>
public class BadRequestException : Exception
{
    public BadRequestException() : base("Bad request")
    {
    }

    public BadRequestException(string message) : base(message)
    {
    }

    public BadRequestException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}

