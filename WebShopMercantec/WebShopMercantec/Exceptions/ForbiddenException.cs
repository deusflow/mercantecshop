namespace WebShopMercantec.Exceptions;

/// <summary>
/// Исключение для запретных операций (недостаточно прав)
/// HTTP Status Code: 403 Forbidden
/// 
/// КОГДА ИСПОЛЬЗОВАТЬ:
/// - Пользователь залогинен, но нет прав на операцию
/// - Попытка доступа к чужим данным
/// - Попытка выполнить admin операцию будучи обычным пользователем
/// 
/// ПРИМЕР:
/// if (order.UserId != currentUserId and not currentUser.IsAdmin)
///     throw new ForbiddenException("You do not have permission to access this order");
/// </summary>
public class ForbiddenException : Exception
{
    public ForbiddenException() : base("Forbidden")
    {
    }

    public ForbiddenException(string message) : base(message)
    {
    }

    public ForbiddenException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}

