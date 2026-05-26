namespace WebShopMercantec.Exceptions;

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

