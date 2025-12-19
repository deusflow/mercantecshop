using WebShopMercantec.Models;
using WebShopMercantec.Shared.DTOs;

namespace WebShopMercantec.Mapping;

public static class StatusLabelMapping
{
    public static StatusLabelDto MapToDto(StatusLabel statusLabel, int assetsCount = 0)
    {
        return new StatusLabelDto
        {
            Id = (int)statusLabel.Id,
            Name = statusLabel.Name ?? "Unknown",
            Color = statusLabel.Color,
            Deployable = statusLabel.Deployable,
            Pending = statusLabel.Pending,
            Archived = statusLabel.Archived,
            Notes = statusLabel.Notes,
            ShowInNav = statusLabel.ShowInNav,
            DefaultLabel = statusLabel.DefaultLabel,
            AssetsCount = assetsCount
        };
    }

    public static IEnumerable<StatusLabelDto> MapToDtos(
        IEnumerable<StatusLabel> statusLabels,
        Func<uint, int>? getAssetsCount = null)
    {
        return statusLabels.Select(s => MapToDto(s, getAssetsCount?.Invoke(s.Id) ?? 0));
    }
}
