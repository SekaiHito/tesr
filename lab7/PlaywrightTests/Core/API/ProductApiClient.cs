using System.Collections.Generic;
using System.Threading.Tasks;
using PlaywrightTests.Core.API;
using Serilog;

namespace PlaywrightTests.Core.API
{
    public class ProductApiClient : BaseApiClient
    {
        public ProductApiClient(ILogger logger, string? baseUrl = null) : base(logger, baseUrl) { }

        /// <summary>
        /// Отримує список товарів. DummyJSON повертає об'єкт з масивом у полі "products".
        /// </summary>
        public async Task<ProductListResponse> GetProductsAsync()
        {
            _logger.Information("Fetching products from DummyJSON");
            return await GetAsync<ProductListResponse>("products");
        }

        /// <summary>
        /// Отримує один товар за ID.
        /// </summary>
        public async Task<Product> GetProductByIdAsync(int id)
        {
            _logger.Information("Fetching product {ProductId} from DummyJSON", id);
            return await GetAsync<Product>($"products/{id}");
        }

        /// <summary>
        /// Створює новий товар.
        /// </summary>
        public async Task<Product> CreateProductAsync(CreateProductRequest request)
        {
            _logger.Information("Creating product: {Title}", request.Title);
            // У DummyJSON ендпоінт для створення - "products/add"
            return await PostAsync<Product>("products/add", request);
        }
    }

    // --- МОДЕЛІ ДАНИХ (DTOs) ---

    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty; // В DummyJSON це Title, а не Name
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Category { get; set; } = string.Empty;
    }

    public class ProductListResponse
    {
        // DummyJSON повертає список у такому форматі
        public List<Product> Products { get; set; } = new();
        public int Total { get; set; }
        public int Skip { get; set; }
        public int Limit { get; set; }
    }

    public class CreateProductRequest
    {
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        // Можна додати інші поля за бажанням
    }
}