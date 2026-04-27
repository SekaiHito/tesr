# API Testing Guide

Comprehensive guide for testing RESTful APIs using the PlaywrightTests framework.

## Getting Started with API Testing

### 1. Create Custom API Client

```csharp
public class UserApiClient : BaseApiClient
{
    public UserApiClient(ILogger logger) : base(logger) { }
    
    public async Task<ApiResponse<User>> GetUserAsync(int id)
    {
        return await GetAsync<ApiResponse<User>>($"api/users/{id}");
    }
    
    public async Task<ApiResponse<User>> CreateUserAsync(CreateUserRequest request)
    {
        return await PostAsync<ApiResponse<User>>("api/users", request);
    }
}
```

### 2. Define Data Transfer Objects

```csharp
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

public class CreateUserRequest
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}
```

### 3. Write API Tests

```csharp
[TestFixture]
[AllureFeature("User API")]
public class UserApiTest : BaseApiTest
{
    [Test]
    [AllureTag("Smoke")]
    public async Task GetUser_ReturnsUserData()
    {
        // Arrange
        var userId = 1;
        
        // Act
        var response = await ProductApiClient.GetUserAsync(userId);
        
        // Assert
        Assert.That(response.Success, Is.True);
        Assert.That(response.Data.Id, Is.EqualTo(userId));
    }
}
```

## HTTP Methods

### GET Request

```csharp
public async Task<T> GetAsync<T>(string endpoint) where T : class
{
    // Simple GET request
    return await GetAsync<T>("api/users/1");
}
```

### POST Request

```csharp
public async Task<T> PostAsync<T>(string endpoint, object data) where T : class
{
    var request = new CreateUserRequest
    {
        Name = "John Doe",
        Email = "john@example.com"
    };
    
    return await PostAsync<T>("api/users", request);
}
```

### PUT Request (Update)

```csharp
public async Task<T> PutAsync<T>(string endpoint, object data) where T : class
{
    var updateRequest = new UpdateUserRequest
    {
        Name = "Jane Doe",
        Email = "jane@example.com"
    };
    
    return await PutAsync<T>("api/users/1", updateRequest);
}
```

### DELETE Request

```csharp
public async Task<T> DeleteAsync<T>(string endpoint) where T : class
{
    return await DeleteAsync<T>("api/users/1");
}
```

## Authentication

### Bearer Token

```csharp
[SetUp]
public void Setup()
{
    ApiClient = new UserApiClient(Logger);
    ApiClient.SetAuthorizationHeader("your_jwt_token_here");
}
```

### Custom Headers

```csharp
public async Task GetWithHeaders()
{
    var headers = new Dictionary<string, string>
    {
        { "X-API-Key", "api-key-value" },
        { "X-Custom-Header", "custom-value" }
    };
    
    return await GetAsync<ApiResponse<User>>("api/users", headers);
}
```

### Basic Authentication

```csharp
public void SetBasicAuth(string username, string password)
{
    var credentials = Convert.ToBase64String(
        Encoding.ASCII.GetBytes($"{username}:{password}")
    );
    ApiClient.SetAuthorizationHeader($"Basic {credentials}");
}
```

## Response Handling

### Successful Response

```csharp
[Test]
public async Task VerifySuccessfulResponse()
{
    var response = await ApiClient.GetUserAsync(1);
    
    Assert.That(response, Is.Not.Null);
    Assert.That(response.Success, Is.True);
    Assert.That(response.Data, Is.Not.Null);
    Assert.That(response.StatusCode, Is.EqualTo(200));
}
```

### Error Response

```csharp
[Test]
public async Task VerifyErrorResponse()
{
    try
    {
        await ApiClient.GetUserAsync(9999); // Non-existent ID
    }
    catch (HttpRequestException ex)
    {
        Assert.That(ex.Message, Does.Contain("404"));
        Logger.Information("Expected error occurred: {Error}", ex.Message);
    }
}
```

## Data Validation

### Validate Response Structure

```csharp
[Test]
public async Task ValidateUserDataStructure()
{
    var response = await ApiClient.GetUserAsync(1);
    var user = response.Data;
    
    Assert.That(user.Id, Is.GreaterThan(0));
    Assert.That(user.Name, Is.Not.Null.And.Not.Empty);
    Assert.That(user.Email, Is.Not.Null.And.Not.Empty);
    Assert.That(user.Email, Does.Match(@"^[^@\s]+@[^@\s]+\.[^@\s]+$"));
}
```

### Validate Response Types

```csharp
[Test]
public async Task ValidateResponseTypes()
{
    var response = await ApiClient.GetUserAsync(1);
    
    Assert.That(response.Data.Id, Is.TypeOf<int>());
    Assert.That(response.Data.Name, Is.TypeOf<string>());
    Assert.That(response.StatusCode, Is.TypeOf<int>());
}
```

## Parameterized Testing

### Using TestCaseSource

```csharp
private static IEnumerable<TestCaseData> UserIds()
{
    yield return new TestCaseData(1).SetName("First user");
    yield return new TestCaseData(2).SetName("Second user");
    yield return new TestCaseData(3).SetName("Third user");
}

[Test, TestCaseSource(nameof(UserIds))]
public async Task GetUser_WithMultipleIds(int userId)
{
    var response = await ApiClient.GetUserAsync(userId);
    Assert.That(response.Success, Is.True);
}
```

## Error Handling

### Handle Network Errors

```csharp
[Test]
public async Task HandleNetworkTimeout()
{
    try
    {
        // This will timeout
        await ApiClient.GetUserAsync(1);
    }
    catch (HttpRequestException ex) when (ex.InnerException is TimeoutException)
    {
        Logger.Error(ex, "Network timeout occurred");
        Assert.Fail("Request timed out");
    }
}
```

### Handle Serialization Errors

```csharp
[Test]
public async Task HandleSerializationError()
{
    try
    {
        // Assuming API returns unexpected response format
        var response = await ApiClient.GetUserAsync(1);
    }
    catch (JsonException ex)
    {
        Logger.Error(ex, "JSON deserialization failed");
        Assert.Fail($"Failed to parse response: {ex.Message}");
    }
}
```

## Performance Testing

### Measure Response Time

```csharp
[Test]
public async Task MeasureApiResponseTime()
{
    var stopwatch = Stopwatch.StartNew();
    var response = await ApiClient.GetUserAsync(1);
    stopwatch.Stop();
    
    Logger.Information("API Response time: {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
    Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(1000), "Response should be < 1s");
}
```

### Load Testing

```csharp
[Test]
public async Task ApiResponseUnderLoad()
{
    var tasks = new List<Task>();
    var stopwatch = Stopwatch.StartNew();
    
    // Send 100 concurrent requests
    for (int i = 0; i < 100; i++)
    {
        tasks.Add(ApiClient.GetUserAsync(1));
    }
    
    await Task.WhenAll(tasks);
    stopwatch.Stop();
    
    Logger.Information("100 requests completed in: {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
    Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(5000));
}
```

## State Management

### Create, Read, Update, Delete (CRUD)

```csharp
[Test]
[Order(1)]
public async Task CreateUser()
{
    var request = new CreateUserRequest
    {
        Name = "Test User",
        Email = "test@example.com"
    };
    
    var response = await ApiClient.CreateUserAsync(request);
    
    Assert.That(response.Success, Is.True);
    DataHelpers.CacheData("created_user_id", response.Data.Id);
}

[Test]
[Order(2)]
public async Task ReadUser()
{
    var userId = DataHelpers.GetCachedData("created_user_id");
    var response = await ApiClient.GetUserAsync((int)userId);
    
    Assert.That(response.Data.Name, Is.EqualTo("Test User"));
}

[Test]
[Order(3)]
public async Task UpdateUser()
{
    var userId = (int)DataHelpers.GetCachedData("created_user_id");
    var updateRequest = new UpdateUserRequest
    {
        Name = "Updated User"
    };
    
    var response = await ApiClient.UpdateUserAsync(userId, updateRequest);
    Assert.That(response.Success, Is.True);
}

[Test]
[Order(4)]
public async Task DeleteUser()
{
    var userId = (int)DataHelpers.GetCachedData("created_user_id");
    var response = await ApiClient.DeleteUserAsync(userId);
    
    Assert.That(response.Success, Is.True);
}
```

## Best Practices

### 1. Use Meaningful Test Names

```csharp
// ✅ Good
[Test]
public async Task GetNonExistentUser_ReturnsNotFoundError()

// ❌ Bad
[Test]
public async Task Test1()
```

### 2. Test Single Responsibility

```csharp
// ✅ Good - Tests one behavior
[Test]
public async Task ValidatUserEmailFormat()
{
    var response = await ApiClient.GetUserAsync(1);
    Assert.That(response.Data.Email, Does.Match(@"^[^@]+@[^@]+\.[^@]+$"));
}

// ❌ Bad - Tests multiple behaviors
[Test]
public async Task GetAndValidateUser()
{
    var response = await ApiClient.GetUserAsync(1);
    Assert.That(response.Success, Is.True);
    Assert.That(response.Data.Id, Is.GreaterThan(0));
    Assert.That(response.Data.Email, Is.Not.Null);
    Assert.That(response.StatusCode, Is.EqualTo(200));
}
```

### 3. Use Descriptive Assertions

```csharp
// ✅ Good - Clear error message
Assert.That(
    response.StatusCode, 
    Is.EqualTo(201), 
    "API should return 201 Created status for new user"
);

// ❌ Bad - Generic error
Assert.That(response.StatusCode, Is.EqualTo(201));
```

### 4. Manage Test Data Lifecycle

```csharp
[SetUp]
public async Task SetupTestData()
{
    // Create necessary test data
    var testUser = await ApiClient.CreateUserAsync(...);
    DataHelpers.CacheData("test_user_id", testUser.Data.Id);
}

[TearDown]
public async Task CleanupTestData()
{
    // Clean up created data
    var userId = DataHelpers.GetCachedData("test_user_id");
    if (userId != null)
    {
        await ApiClient.DeleteUserAsync((int)userId);
    }
}
```

### 5. Use Environment Variables for URLs

```csharp
public class UserApiClient : BaseApiClient
{
    public UserApiClient(ILogger logger) 
        : base(logger, ConfigManager.Environment.ApiUrl) { }
}
```

## Troubleshooting

### Common Issues

| Issue | Solution |
|-------|----------|
| 401 Unauthorized | Verify authentication token/credentials |
| 403 Forbidden | Check user permissions and access levels |
| 404 Not Found | Verify endpoint URL and resource ID |
| 500 Server Error | Check server logs, retry test |
| Timeout | Increase timeout or check network |
| SSL Certificate Error | Use proper certificates or disable validation for test only |

---

**Version**: 1.0  
**Last Updated**: 2026-04-27
