using WebShopMercantec.Models;
using WebShopMercantec.Shared.DTOs;

namespace WebShopMercantec.Mapping;

public static class OrderMapping
{
    public static OrderDto MapToDto(CheckoutRequest order, User? user = null, string? productName = null)
    {
        var status = order switch
        {
            { FulfilledAt: not null }                => "Fulfilled",
            { CanceledAt: not null }                 => "Canceled",
            _                                        => "Pending"
        };

        return new OrderDto
        {
            Id = (int)order.Id,
            UserId = order.UserId,
            UserName = user != null
                ? $"{user.FirstName} {user.LastName}".Trim().IfEmpty(user.Username ?? "Unknown")
                : "Unknown",
            RequestableId = order.RequestableId,
            RequestableType = order.RequestableType,
            ProductName = productName ?? $"#{order.RequestableId}",
            Quantity = order.Quantity,
            Status = status,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            FulfilledAt = order.FulfilledAt,
            CanceledAt = order.CanceledAt
        };
    }
}

file static class StringExtensions
{
    public static string IfEmpty(this string? s, string fallback) =>
        string.IsNullOrWhiteSpace(s) ? fallback : s;
}

