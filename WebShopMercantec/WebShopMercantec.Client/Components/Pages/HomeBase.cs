using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using WebShopMercantec.Shared.DTOs;

namespace WebShopMercantec.Client.Components.Pages;

public class HomeBase : ComponentBase, IDisposable
{
    [Inject]
    protected HttpClient Http { get; set; } = default!;

    public readonly CancellationTokenSource Cts = new();
    public readonly List<ProductDto> Items = new();
    public readonly List<CategoryDto> Categories = new();
    public bool IsLoading;
    public string? Error;
    public int Page = 1;
    public int PageSize = 20;
    public int TotalPages = 1;
    public string SearchText = string.Empty;
    public int? SelectedCategoryId;

    protected override async Task OnInitializedAsync()
    {
        await LoadCategoriesAsync();
        await LoadAsync();
    }

    public async Task SelectCategoryAsync(int? categoryId)
    {
        SelectedCategoryId = categoryId;
        Page = 1;
        await LoadAsync();
    }

    private async Task LoadCategoriesAsync()
    {
        try
        {
            Categories.Clear();
            var categories = await Http.GetFromJsonAsync<List<CategoryDto>>("/api/categories/catalog", Cts.Token);
            if (categories != null)
                Categories.AddRange(categories.OrderByDescending(c => c.ItemsCount).ThenBy(c => c.Name));
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
    }

    public async Task OnSearchKeyDown(KeyboardEventArgs args)
    {
        if (args.Key == "Enter")
        {
            await SearchAsync();
        }
    }

    public async Task SearchAsync()
    {
        Page = 1;
        await LoadAsync();
    }

    public async Task ClearSearchAsync()
    {
        SearchText = string.Empty;
        Page = 1;
        await LoadAsync();
    }

    public async Task NextPageAsync()
    {
        if (Page < TotalPages)
        {
            Page++;
            await LoadAsync();
        }
    }

    public async Task PrevPageAsync()
    {
        if (Page > 1)
        {
            Page--;
            await LoadAsync();
        }
    }

    protected async Task LoadAsync()
    {
        IsLoading = true;
        Error = null;
        Items.Clear();

        try
        {
            var url = $"/api/products/paged?page={Page}&pageSize={PageSize}&search={Uri.EscapeDataString(SearchText)}";
            if (SelectedCategoryId.HasValue)
                url += $"&categoryId={SelectedCategoryId.Value}";

            var response = await Http.GetFromJsonAsync<ProductsPagedResponse>(url, Cts.Token);

            if (response?.Items is not null)
            {
                Items.AddRange(response.Items);
                TotalPages = Math.Max(1, response.TotalPages);
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    public static string GetDescription(ProductDto item)
    {
        if (!string.IsNullOrWhiteSpace(item.Notes))
        {
            return item.Notes;
        }

        return string.Join(", ",
            new[] { item.ModelName, item.CategoryName, item.ManufacturerName }
                .Where(x => !string.IsNullOrWhiteSpace(x)));
    }

    public static string GetProductHref(int id)
    {
        return $"/product/{id}";
    }

    public void Dispose()
    {
        Cts.Cancel();
        Cts.Dispose();
    }

    protected sealed class ProductsPagedResponse
    {
        public List<ProductDto> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
