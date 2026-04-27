using System.Collections.Generic;
using System.Threading.Tasks;
using PlaywrightTests.Core.API;
using Serilog;

namespace PlaywrightTests.Core.API
{
    /// <summary>
    /// Example API client for product-related endpoints.
    /// </summary>
    public class ProductApiClient : BaseApiClient
    {
        public ProductApiClient(ILogger logger, string baseUrl = null) : base(logger, baseUrl) { }

        /// <summary>
        /// Gets all products from API.
        /// </summary>
        public async Task<ApiResponse<List<Product>>> GetProductsAsync()
        {
            _logger.Information("Fetching products from API");
            return await GetAsync<ApiResponse<List<Product>>>("api/products");
        }

        /// <summary>
        /// Gets product by ID.
        /// </summary>
        public async Task<ApiResponse<Product>> GetProductByIdAsync(int id)
        {
            _logger.Information("Fetching product {ProductId} from API", id);
            return await GetAsync<ApiResponse<Product>>($"api/products/{id}");
        }

        /// <summary>
        /// Creates new product.
        /// </summary>
        public async Task<ApiResponse<Product>> CreateProductAsync(CreateProductRequest request)
        {
            _logger.Information("Creating product: {ProductName}", request.Name);
            return await PostAsync<ApiResponse<Product>>("api/products", request);
        }

        /// <summary>
        /// Updates product.
        /// </summary>
        public async Task<ApiResponse<Product>> UpdateProductAsync(int id, UpdateProductRequest request)
        {
            _logger.Information("Updating product {ProductId}", id);
            return await PutAsync<ApiResponse<Product>>($"api/products/{id}", request);
        }

        /// <summary>
        /// Deletes product.
        /// </summary>
        public async Task<ApiResponse<object>> DeleteProductAsync(int id)
        {
            _logger.Information("Deleting product {ProductId}", id);
            return await DeleteAsync<ApiResponse<object>>($"api/products/{id}");
        }
    }

    // DTOs
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Category { get; set; }
    }

    public class CreateProductRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Category { get; set; }
    }

    public class UpdateProductRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Category { get; set; }
    }

    public class ApiResponse<T> where T : class
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public int StatusCode { get; set; }
    }
}
