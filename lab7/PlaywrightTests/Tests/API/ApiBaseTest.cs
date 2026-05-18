using NUnit.Framework;
using Serilog;
using PlaywrightTests.Core.Managers;
using PlaywrightTests.Config;
using PlaywrightTests.Core.API;

namespace PlaywrightTests.Tests.API
{
    public class ApiBaseTest
    {
        protected ILogger Logger;
        protected ProductApiClient ProductApi;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            // Використовуємо ваш LoggerManager
            LoggerManager.Initialize(ConfigManager.Logging.MinimumLevel);
            Logger = LoggerManager.Logger;
        }

        [SetUp]
        public void Setup()
        {
            Logger.Information("--- Starting API Test: {TestName} ---", TestContext.CurrentContext.Test.Name);
            
            // Ініціалізація клієнта (URL береться з appsettings.json через ConfigManager)
            ProductApi = new ProductApiClient(Logger);
        }

        [TearDown]
        public void TearDown()
        {
            ProductApi?.Dispose();
        }
    }
}