using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;

namespace PlaywrightTests.Core.Helpers
{
    /// <summary>
    /// Provides helper methods for test data management and generation.
    /// Handles data caching, random generation, and test data utilities.
    /// </summary>
    public class DataHelpers
    {
        private static readonly Dictionary<string, object> _dataCache = new();
        private readonly ILogger _logger;
        private static readonly Random _random = new();

        public DataHelpers(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Caches test data by key for reuse across tests.
        /// </summary>
        public void CacheData(string key, object data)
        {
            _dataCache[key] = data;
            _logger.Debug("Data cached: {Key}", key);
        }

        /// <summary>
        /// Retrieves cached test data by key.
        /// </summary>
        public object GetCachedData(string key)
        {
            if (_dataCache.TryGetValue(key, out var data))
            {
                _logger.Debug("Retrieved cached data: {Key}", key);
                return data;
            }
            _logger.Warning("Cached data not found: {Key}", key);
            return null;
        }

        /// <summary>
        /// Retrieves cached test data as specific type.
        /// </summary>
        public T GetCachedData<T>(string key) where T : class
        {
            var data = GetCachedData(key);
            return data as T;
        }

        /// <summary>
        /// Clears all cached data.
        /// </summary>
        public void ClearCache()
        {
            _dataCache.Clear();
            _logger.Debug("Data cache cleared");
        }

        /// <summary>
        /// Generates a random string of specified length.
        /// </summary>
        public string GenerateRandomString(int length = 10, string charset = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789")
        {
            var result = new string(Enumerable.Range(0, length)
                .Select(_ => charset[_random.Next(charset.Length)])
                .ToArray());
            _logger.Debug("Generated random string of length: {Length}", length);
            return result;
        }

        /// <summary>
        /// Generates a random email address.
        /// </summary>
        public string GenerateRandomEmail()
        {
            string email = $"test_{GenerateRandomString(8)}@example.com";
            _logger.Debug("Generated random email: {Email}", email);
            return email;
        }

        /// <summary>
        /// Generates a random phone number.
        /// </summary>
        public string GenerateRandomPhoneNumber(string format = "+{0}-{1}-{2}")
        {
            string countryCode = _random.Next(1, 100).ToString();
            string areaCode = _random.Next(100, 999).ToString();
            string number = _random.Next(1000000, 9999999).ToString();
            string phone = string.Format(format, countryCode, areaCode, number);
            _logger.Debug("Generated random phone: {Phone}", phone);
            return phone;
        }

        /// <summary>
        /// Generates a random date within a range.
        /// </summary>
        public DateTime GenerateRandomDate(DateTime minDate, DateTime maxDate)
        {
            int range = (int)(maxDate - minDate).TotalDays;
            DateTime randomDate = minDate.AddDays(_random.Next(range));
            _logger.Debug("Generated random date: {Date}", randomDate);
            return randomDate;
        }

        /// <summary>
        /// Generates a random number within a range.
        /// </summary>
        public int GenerateRandomNumber(int minValue, int maxValue)
        {
            int number = _random.Next(minValue, maxValue + 1);
            _logger.Debug("Generated random number: {Number} (range: {Min}-{Max})", number, minValue, maxValue);
            return number;
        }

        /// <summary>
        /// Masks sensitive data (passwords, tokens, etc.) in strings.
        /// </summary>
        public string MaskSensitiveData(string data, string prefix = "", int visibleChars = 2)
        {
            if (string.IsNullOrEmpty(data) || data.Length <= visibleChars)
                return "***";

            string visible = data.Substring(0, visibleChars);
            return $"{prefix}{visible}***";
        }

        /// <summary>
        /// Validates email format.
        /// </summary>
        public bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Generates test user credentials.
        /// </summary>
        public (string username, string password) GenerateTestCredentials()
        {
            string username = GenerateRandomEmail();
            string password = GenerateRandomString(12, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%");
            _logger.Debug("Generated test credentials for user: {Username}", username);
            return (username, password);
        }

        /// <summary>
        /// Validates if a string matches a pattern.
        /// </summary>
        public bool ValidatePattern(string value, string pattern)
        {
            try
            {
                var regex = new System.Text.RegularExpressions.Regex(pattern);
                bool matches = regex.IsMatch(value);
                _logger.Debug("Pattern validation for: {Value} against {Pattern}: {Result}", value, pattern, matches);
                return matches;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error validating pattern");
                return false;
            }
        }
    }
}
