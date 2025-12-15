namespace WebShopMercantec.Exceptions;

/// <summary>
/// Исключение для неавторизованных запросов
/// HTTP Status Code: 401 Unauthorized
/// 
/// КОГДА ИСПОЛЬЗОВАТЬ:
/// - Неверные credentials (email/password)
/// - Токен истек или невалидный
/// - Пользователь не залогинен
/// 
/// ПРИМЕР:
/// var user = await _unitOfWork.Users.GetByEmailAsync(email);
/// if (user == null || !VerifyPassword(password, user.Password))
///     throw new UnauthorizedException("Invalid email or password");
/// </summary>
public class UnauthorizedException : Exception
{
    public UnauthorizedException() : base("Unauthorized")
    {
    }

    public UnauthorizedException(string message) : base(message)
    {
    }

    public UnauthorizedException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}

