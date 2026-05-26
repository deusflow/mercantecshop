namespace WebShopMercantec.Exceptions;

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

