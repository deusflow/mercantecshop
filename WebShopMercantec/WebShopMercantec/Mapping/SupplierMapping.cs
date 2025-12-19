using WebShopMercantec.Models;
using WebShopMercantec.Shared.DTOs;

namespace WebShopMercantec.Mapping;

public static class SupplierMapping
{
    public static SupplierDto MapToDto(Supplier supplier)
    {
        return new SupplierDto
        {
            Id = (int)supplier.Id,
            Name = supplier.Name ?? "Unknown",
            Address = supplier.Address,
            City = supplier.City,
            State = supplier.State,
            Country = supplier.Country,
            Zip = supplier.Zip,
            Contact = supplier.Contact,
            Phone = supplier.Phone,
            Fax = supplier.Fax,
            Email = supplier.Email,
            Url = supplier.Url,
            Notes = supplier.Notes
        };
    }

    public static IEnumerable<SupplierDto> MapToDtos(IEnumerable<Supplier> suppliers)
    {
        return suppliers.Select(MapToDto);
    }
}

