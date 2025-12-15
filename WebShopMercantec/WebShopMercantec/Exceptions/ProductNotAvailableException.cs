namespace WebShopMercantec.Exceptions;

/// <summary>
/// Исключение для случаев, когда товар/аксессуар недоступен для заказа
/// HTTP Status Code: 409 Conflict (или 400 Bad Request)
/// 
/// КОГДА ИСПОЛЬЗОВАТЬ:
/// - Товар уже назначен кому-то
/// - Товар в архиве
/// - Товар не requestable
/// - Аксессуар закончился (qty = 0)
/// - Попытка заказать товар повторно
/// 
/// ПРИМЕР:
/// var isAvailable = await _unitOfWork.Products.IsAvailableForCheckoutAsync(productId);
/// if (!isAvailable)
///     throw new ProductNotAvailableException("Product is not available for checkout");
/// </summary>
public class ProductNotAvailableException : Exception
{
    public int? ProductId { get; }
    public string? Reason { get; }

    public ProductNotAvailableException() 
        : base("Product is not available")
    {
    }

    public ProductNotAvailableException(string message) : base(message)
    {
    }

    public ProductNotAvailableException(int productId, string reason)
        : base($"Product {productId} is not available: {reason}")
    {
        ProductId = productId;
        Reason = reason;
    }

    public ProductNotAvailableException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}

