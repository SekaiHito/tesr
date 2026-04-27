using System.Threading.Tasks;
using NUnit.Framework;
using Serilog;
using PlaywrightTests.Core.API;

namespace PlaywrightTests.Tests.API
{
    /// <summary>
    /// Base class for API tests.
    /// Provides common setup and teardown for API testing.
    /// </summary>
    public class BaseApiTest
    {
        protected ILogger Logger;
        protected ProductApiClient ProductApiClient;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            // Initialize logger once for all tests
            PlaywrightTests.Core.Managers.LoggerManager.Initialize("Information");
            Logger = PlaywrightTests.Core.Managers.LoggerManager.Logger;

            Logger.Information("==== API Test Execution Started ====");
            Logger.Information("Base URL: {BaseUrl}", PlaywrightTests.Config.ConfigManager.Environment.ApiUrl);
        }

        [SetUp]
        public void Setup()
        {
            Logger.Information("Test Setup: {TestName}", TestContext.CurrentContext.Test.Name);

            try
            {
                // Initialize API clients
                ProductApiClient = new ProductApiClient(Logger);

                Logger.Information("API Test Setup completed successfully");
            }
            catch (System.Exception ex)
            {
                Logger.Error(ex, "API Test Setup failed");
                throw;
            }
        }

        [TearDown]
        public void Teardown()
        {
            Logger.Information("Test Teardown: {TestName}", TestContext.CurrentContext.Test.Name);

            try
            {
                // Cleanup API resources
                ProductApiClient?.Dispose();

                if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
                {
                    Logger.Error("Test Failed: {TestName}", TestContext.CurrentContext.Test.Name);
                }
                else
                {
                    Logger.Information("Test Passed: {TestName}", TestContext.CurrentContext.Test.Name);
                }

                Logger.Information("API Test Teardown completed");
            }
            catch (System.Exception ex)
            {
                Logger.Error(ex, "Error during API test teardown");
            }
        }

        [OneTimeTearDown]
        public void OneTimeTeardown()
        {
            Logger.Information("==== API Test Execution Completed ====");
            PlaywrightTests.Core.Managers.LoggerManager.Shutdown();
        }
    }
}
