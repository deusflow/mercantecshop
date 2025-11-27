namespace WebShopMercantec.DTOs.Orders;

/// <summary>
/// Возможные статусы заказа.
/// </summary>
public enum OrderStatus
{
    Draft,
    PendingPayment,
    Paid,
    Processing,
    Shipped,
    Delivered,
    Cancelled,
    Refunded
}

