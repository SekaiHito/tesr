namespace PlaywrightTests.Tests;

using Allure.Net.Commons;
using PlaywrightTests.Core.Config;
using PlaywrightTests.Core.Managers;

[SetUpFixture]
public class TestSetup
{
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        AllureLifecycle.Instance.CleanupResultDirectory("allure-results");
    }
}

public abstract class BaseTest
{
    protected BrowserManager BrowserManager => BrowserManager.Instance;
    protected LoggerManager Logger => LoggerManager.Instance;
    protected ConfigManager Config => ConfigManager.Instance;

    [SetUp]
    public virtual async Task TestSetUpAsync()
    {
        try
        {
            Logger.Info("========== TEST START: {TestName} ==========", TestContext.CurrentContext.Test.Name);
            Logger.Info("Test method: {MethodName}", TestContext.CurrentContext.Test.MethodName);

            await BrowserManager.InitializeAsync().ConfigureAwait(false);
            Logger.Info("Browser and page initialized for test");
        }
        catch (Exception ex)
        {
            Logger.Fatal(ex, "Test setup failed");
            throw;
        }
    }

    [TearDown]
    public virtual async Task TestTearDownAsync()
    {
        try
        {
            var testStatus = TestContext.CurrentContext.Result.Outcome.Status;
            Logger.Info("Test result: {Status}", testStatus);

            if (testStatus == NUnit.Framework.Interfaces.TestStatus.Failed)
            {
                Logger.Error("Test FAILED: {TestName}", TestContext.CurrentContext.Test.Name);

                // Take screenshot on failure
                if (Config.Framework.TestData.ScreenshotsOnFailure)
                {
                    var screenshotPath = await BrowserManager.TakeScreenshotAsync($"FAILED_{TestContext.CurrentContext.Test.Name}").ConfigureAwait(false);
                    Logger.Info("Failure screenshot saved: {Path}", screenshotPath);

                    // Attach to Allure report
                    if (!string.IsNullOrEmpty(screenshotPath) && File.Exists(screenshotPath))
                    {
                        AllureApi.AddAttachment("Failure Screenshot", "image/png", screenshotPath);
                    }
                }
            }
            else
            {
                Logger.Info("Test PASSED: {TestName}", TestContext.CurrentContext.Test.Name);
            }

            await BrowserManager.CloseAsync().ConfigureAwait(false);
            Logger.Info("========== TEST END: {TestName} ==========", TestContext.CurrentContext.Test.Name);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Error during test teardown");
        }
        finally
        {
            LoggerManager.Reset();
        }
    }

    /// <summary>
    /// Gets test name for logging
    /// </summary>
    protected string GetTestName()
    {
        return TestContext.CurrentContext.Test.Name;
    }

    /// <summary>
    /// Gets test class name
    /// </summary>
    protected string GetTestClassName()
    {
        return TestContext.CurrentContext.Test.ClassName ?? string.Empty;
    }

    /// <summary>
    /// Adds Allure feature
    /// </summary>
    protected void AddAllureFeature(string feature)
    {
        AllureApi.Instance.AddFeature(feature);
        Logger.Info("Allure feature: {Feature}", feature);
    }

    /// <summary>
    /// Adds Allure story
    /// </summary>
    protected void AddAllureStory(string story)
    {
        AllureApi.Instance.AddStory(story);
        Logger.Info("Allure story: {Story}", story);
    }

    /// <summary>
    /// Adds Allure severity
    /// </summary>
    protected void AddAllureSeverity(SeverityLevel severity)
    {
        AllureApi.Instance.AddSeverity(severity);
        Logger.Info("Allure severity: {Severity}", severity);
    }

    /// <summary>
    /// Adds Allure description
    /// </summary>
    protected void AddAllureDescription(string description)
    {
        AllureApi.Instance.AddDescription(description);
        Logger.Info("Test description: {Description}", description);
    }

    /// <summary>
    /// Adds Allure step
    /// </summary>
    protected void AddAllureStep(string step)
    {
        AllureApi.Instance.WrapInStep(() => { }, step);
        Logger.Debug("Allure step: {Step}", step);
    }

    /// <summary>
    /// Adds Allure attachment
    /// </summary>
    protected void AddAllureAttachment(string name, string filePath, string mimeType = "text/plain")
    {
        if (File.Exists(filePath))
        {
            AllureApi.AddAttachment(name, mimeType, filePath);
            Logger.Info("Allure attachment added: {Name}", name);
        }
    }
}
