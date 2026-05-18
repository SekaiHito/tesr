using NUnit.Framework;
using Moq;
using Serilog;
using PlaywrightTests.Core.Helpers;
using PlaywrightTests.Core.Managers;
using System;

namespace PlaywrightTests.UnitTests
{
    [TestFixture]
    public class FrameworkUnitTests
    {
        private Mock<ILogger> _loggerMock;
        private DataHelpers _dataHelpers;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger>();
            _dataHelpers = new DataHelpers(_loggerMock.Object);
        }

        #region DataHelpers Tests

        [Test]
        [Category("Unit")]
        public void GenerateRandomEmail_ShouldReturnValidEmailFormat()
        {
            // Act
            var email = _dataHelpers.GenerateRandomEmail();

            // Assert
            Assert.That(_dataHelpers.IsValidEmail(email), Is.True, $"Generated email '{email}' should be valid.");
        }

        [Test]
        [Category("Unit")]
        public void MaskSensitiveData_ShouldReturnMaskedString()
        {
            // Arrange
            string token = "secret_token_123";

            // Act
            var masked = _dataHelpers.MaskSensitiveData(token, visibleChars: 4);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(masked, Does.StartWith("secr"));
                Assert.That(masked, Does.Contain("***"));
                Assert.That(masked.Length, Is.LessThan(token.Length));
            });
        }

        [Test]
        [Category("Unit")]
        public void CacheData_ShouldRetrieveCorrectObject()
        {
            // Arrange
            var testData = new { Name = "TestItem", Id = 101 };
            string key = "item_key";

            // Act
            _dataHelpers.CacheData(key, testData);
            var retrieved = _dataHelpers.GetCachedData(key);

            // Assert
            Assert.That(retrieved, Is.EqualTo(testData));
        }

        #endregion

        #region WaitHelpers Logic Tests (Retry Logic)

        [Test]
        [Category("Unit")]
        public async Task RetryAsync_ShouldSucceedAfterFailures()
        {
            // Arrange
            int attempts = 0;
            Func<Task<string>> unstableAction = async () =>
            {
                attempts++;
                if (attempts < 2) throw new Exception("Temporary failure");
                return await Task.FromResult("Success");
            };

            // Act
            // Використовуємо WaitHelpers для перевірки логіки ретраїв
            var waitHelpers = new WaitHelpers(new Mock<Microsoft.Playwright.IPage>().Object, _loggerMock.Object);
            var result = await waitHelpers.RetryAsync(unstableAction, maxAttempts: 3);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo("Success"));
                Assert.That(attempts, Is.EqualTo(2));
            });
        }

        #endregion

        #region Validation Logic Tests

        [TestCase("valid@test.com", true)]
        [TestCase("invalid-email", false)]
        [TestCase("", false)]
        public void IsValidEmail_CheckVariousFormats(string email, bool expected)
        {
            Assert.That(_dataHelpers.IsValidEmail(email), Is.EqualTo(expected));
        }

        #endregion
    }
}