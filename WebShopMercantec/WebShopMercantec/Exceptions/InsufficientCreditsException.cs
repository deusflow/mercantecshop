namespace WebShopMercantec.Exceptions;

/// <summary>
/// Исключение для случаев, когда у пользователя недостаточно кредитов
/// HTTP Status Code: 402 Payment Required (или 400 Bad Request)
/// 
/// КОГДА ИСПОЛЬЗОВАТЬ:
/// - Попытка создать заказ без достаточного количества кредитов
/// - Попытка купить товар дороже, чем баланс
/// 
/// ПРИМЕР:
/// if (user.AvailableCredits is less than productPrice)
///     throw new InsufficientCreditsException(required: productPrice, available: user.AvailableCredits);
/// </summary>
public class InsufficientCreditsException : Exception
{
    public decimal Required { get; }
    public decimal Available { get; }

    public InsufficientCreditsException() 
        : base("Insufficient credits")
    {
    }

    public InsufficientCreditsException(string message) : base(message)
    {
    }

    public InsufficientCreditsException(decimal required, decimal available)
        : base($"Insufficient credits. Required: {required}, Available: {available}")
    {
        Required = required;
        Available = available;
    }

    public InsufficientCreditsException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}

