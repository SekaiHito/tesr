using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using Allure.NUnit.Attributes;
using PlaywrightTests.Core.API;

namespace PlaywrightTests.Tests.API
{
    /// <summary>
    /// Product API test suite.
    /// Tests product API endpoints and data validation.
    /// </summary>
    [TestFixture]
    [AllureSuite("API")]
    [AllureFeature("Product API")]
    public class ProductApiTest : BaseApiTest
    {
        /// <summary>
        /// Test: Verify get products API endpoint.
        /// </summary>
        [Test]
        [AllureTag("Smoke")]
        [Category("Smoke")]
        [Category("API")]
        public async Task GetProducts_ReturnsSuccessfulResponse()
        {
            Logger.Information("Test: GetProducts_ReturnsSuccessfulResponse");

            var response = await ProductApiClient.GetProductsAsync();

            Assert.That(response, Is.Not.Null, "Response should not be null");
            Assert.That(response.Success, Is.True, "Response should indicate success");
            Assert.That(response.Data, Is.Not.Null, "Product data should be returned");
            Assert.That(response.Data.Count, Is.GreaterThanOrEqualTo(0), "Product list should be valid");

            Logger.Information("Test passed: Retrieved {Count} products", response.Data.Count);
        }

        /// <summary>
        /// Test: Verify get product by ID API endpoint.
        /// </summary>
        [Test]
        [AllureTag("Regression")]
        [Category("Regression")]
        [Category("API")]
        public async Task GetProductById_ReturnsProduct()
        {
            Logger.Information("Test: GetProductById_ReturnsProduct");

            const int productId = 1;
            var response = await ProductApiClient.GetProductByIdAsync(productId);

            Assert.That(response, Is.Not.Null, "Response should not be null");
            if (response.Success)
            {
                Assert.That(response.Data, Is.Not.Null, "Product data should be returned");
                Assert.That(response.Data.Id, Is.EqualTo(productId), "Product ID should match request");
            }

            Logger.Information("Test passed: Retrieved product {ProductId}", productId);
        }

        /// <summary>
        /// Test: Verify create product API endpoint.
        /// </summary>
        [Test]
        [AllureTag("Critical")]
        [Category("Critical")]
        [Category("API")]
        public async Task CreateProduct_ReturnsCreatedProduct()
        {
            Logger.Information("Test: CreateProduct_ReturnsCreatedProduct");

            var createRequest = new CreateProductRequest
            {
                Name = "Test Product",
                Description = "Automated test product",
                Price = 99.99m,
                Stock = 100,
                Category = "Test"
            };

            var response = await ProductApiClient.CreateProductAsync(createRequest);

            Assert.That(response, Is.Not.Null, "Response should not be null");
            if (response.Success)
            {
                Assert.That(response.Data, Is.Not.Null, "Created product should be returned");
                Assert.That(response.Data.Name, Is.EqualTo(createRequest.Name), "Product name should match request");
            }

            Logger.Information("Test passed: Product created with name: {ProductName}", createRequest.Name);
        }
    }
}
