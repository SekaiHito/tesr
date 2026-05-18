using NUnit.Framework;
using System.Threading.Tasks;
using PlaywrightTests.Core.API;

namespace PlaywrightTests.Tests.API
{
    [TestFixture]
    [Category("API")]
    public class ProductApiTests : ApiBaseTest
    {
        [Test]
        public async Task GetProducts_ShouldReturnListFromDummyJson()
        {
            var response = await ProductApi.GetProductsAsync();

            Assert.Multiple(() =>
            {
                Assert.That(response.Products, Is.Not.Null);
                Assert.That(response.Products.Count, Is.GreaterThan(0));
                Assert.That(response.Products[0].Title, Is.Not.Null.Or.Empty);
            });
        }

        [Test]
        public async Task CreateProduct_ShouldReturnObjectWithId()
        {
            var request = new CreateProductRequest { Title = "Test Product", Price = 100 };
            var result = await ProductApi.CreateProductAsync(request);

            Assert.Multiple(() =>
            {
                Assert.That(result.Id, Is.GreaterThan(0));
                Assert.That(result.Title, Is.EqualTo(request.Title));
            });
        }
    }
}