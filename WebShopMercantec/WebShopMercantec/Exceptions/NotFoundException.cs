namespace WebShopMercantec.Exceptions;

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

