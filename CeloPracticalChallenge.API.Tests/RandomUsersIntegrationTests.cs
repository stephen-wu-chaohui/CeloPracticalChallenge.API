using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using CeloPracticalChallenge.API.DTOs;

namespace CeloPracticalChallenge.API.Tests
{
    public class RandomUsersIntegrationTests : IClassFixture<TestWebFactory>
    {
        private readonly TestWebFactory _factory;

        public RandomUsersIntegrationTests(TestWebFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task List_NoParameters_Returns100Users()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/randomusers");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("application/json", response.Content.Headers.ContentType.ToString());
            var content = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrWhiteSpace(content));

            var users = JsonSerializer.Deserialize<IList<RandomUserDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.Equal(100, users.Count);
        }

        [Fact]
        public async Task List_Results20_Returns20Users()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/randomusers?results=20");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("application/json", response.Content.Headers.ContentType.ToString());
            var content = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrWhiteSpace(content));

            var users = JsonSerializer.Deserialize<IList<RandomUserDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.Equal(20, users.Count);
        }

        [Fact]
        public async Task List_FirstNameIsJessica_ReturnsJessica()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/randomusers?firstName=Jessica");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("application/json", response.Content.Headers.ContentType.ToString());
            var content = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrWhiteSpace(content));

            var users = JsonSerializer.Deserialize<IList<RandomUserDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.All(users, user => Assert.Equal("Jessica", user.Name.First));
        }

        [Fact]
        public async Task List_LastNameIsChen_ReturnsChen()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/randomusers?lastName=Chen");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("application/json", response.Content.Headers.ContentType.ToString());
            var content = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrWhiteSpace(content));

            var users = JsonSerializer.Deserialize<IList<RandomUserDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.All(users, user => Assert.Equal("Chen", user.Name.Last));
        }

        [Fact]
        public async Task List_SearchJessicaGreen_ReturnsOne()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/randomusers?lastName=Clarke&firstName=Eli&results=5");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("application/json", response.Content.Headers.ContentType.ToString());
            var content = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrWhiteSpace(content));

            var users = JsonSerializer.Deserialize<IList<RandomUserDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.Single(users);
            Assert.All(users, user => {
                Assert.Equal("Eli", user.Name.First);
                Assert.Equal("Clarke", user.Name.Last);
            });
        }

        [Fact]
        public async Task List_SearchJessicaGreen_ReturnsNone()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/randomusers?lastName=Nobody&firstName=Eli&results=5");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("application/json", response.Content.Headers.ContentType.ToString());
            var content = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrWhiteSpace(content));

            var users = JsonSerializer.Deserialize<IList<RandomUserDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.Empty(users);
        }

        [Fact]
        public async Task Get_ExistingId_ReturnsOK()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var ExistingId = await FirstExistingIdAsync();
            var response = await client.GetAsync($"/api/randomusers/{ExistingId}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("application/json", response.Content.Headers.ContentType.ToString());
            var content = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrWhiteSpace(content));

            var user = JsonSerializer.Deserialize<RandomUserDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.Equal(ExistingId, user.Id);
        }

        [Fact]
        public async Task Get_NonExistingId_ReturnsNotFound()
        {
            const int NonExistingId = 9999999;
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync($"/api/randomusers/{NonExistingId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var details = JsonSerializer.Deserialize<ProblemDetails>(content, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy .CamelCase});
            Assert.Equal(404, details.Status);
        }

        [Fact]
        public async Task Put_ExistingId_ReturnsNoContent()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var user = new DTOs.RandomUserForUpdateDto {
                Email = "new.user@example.com",
                Name = new NameDto {
                    Title = "Mr",
                    First = "New",
                    Last = "User",
                },
                DateOfBirth = new DateTime(1990, 1, 1),
                PhoneNumber = "(204)-997-2604",
            };

            var ExistingId = await FirstExistingIdAsync();
            var jsonString = JsonSerializer.Serialize(user);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"/api/randomusers/{ExistingId}", httpContent);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task Put_IllegalData_ReturnsUnprocessable()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var ExistingId = await FirstExistingIdAsync();
            var user = new DTOs.RandomUserForUpdateDto {
                Name = new NameDto {
                    Title = "Mr",
                    First = "New",
                    Last = "User",
                },
                DateOfBirth = new DateTime(1990, 1, 1),
                PhoneNumber = "(204)-997-2604",
            };

            var jsonString = JsonSerializer.Serialize(user);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"/api/randomusers/{ExistingId}", httpContent);

            // Assert
            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        }

        [Fact]
        public async Task Put_NonExistingId_ReturnsNotFound()
        {
            const int NonExistingId = 999999;

            // Arrange
            var client = _factory.CreateClient();

            // Act
            var user = new DTOs.RandomUserForUpdateDto {
                Email = "new.user@example.com",
                Name = new NameDto {
                    Title = "Mr",
                    First = "New",
                    Last = "User",
                },
                DateOfBirth = new DateTime(1990, 1, 1),
                PhoneNumber = "(204)-997-2604",
            };

            var jsonString = JsonSerializer.Serialize(user);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"/api/randomusers/{NonExistingId}", httpContent);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async void Put_UnsafeButIdempotent()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var Existing = await FirstExistingAsync();
            var oldEmail = Existing.Email;

            var newEmail = $"{Guid.NewGuid()}@somemail.com";

            var updateValue = new DTOs.RandomUserForUpdateDto {
                Email = newEmail,
                Name = new NameDto {
                    Title = "Mr",
                    First = "New",
                    Last = "User",
                },
                DateOfBirth = new DateTime(1990, 1, 1),
                PhoneNumber = "(204)-997-2604",
            };

            var jsonString = JsonSerializer.Serialize(updateValue);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"/api/randomusers/{Existing.Id}", httpContent);
            var responseAgain = await client.PutAsync($"/api/randomusers/{Existing.Id}", httpContent);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            Assert.Equal(HttpStatusCode.NoContent, responseAgain.StatusCode);

            var newValue = await GetExistingAsync(Existing.Id);
            Assert.NotEqual(newValue.Email, oldEmail);
            Assert.Equal(newValue.Email, newEmail);
        }

        [Fact]
        public async Task Patch_ExistingId_ReturnsNoContent()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var ExistingId = await FirstExistingIdAsync();
            var cmd = new[] { new PatchCommand("replace", "/email", "new@somemail.com") };
            var jsonString = JsonSerializer.Serialize(cmd, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json-patch+json");
            var response = await client.PatchAsync($"/api/randomusers/{ExistingId}", httpContent);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task Patch_IllegalData_ReturnsUnprocessable()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var cmd = new[] { new PatchCommand("remove", "/email") };
            var ExistingId = await FirstExistingIdAsync();
            var jsonString = JsonSerializer.Serialize(cmd, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json-patch+json");

            var response = await client.PatchAsync($"/api/randomusers/{ExistingId}", httpContent);

            // Assert
            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        }

        [Fact]
        public async Task Patch_NonExistingId_ReturnsNotFound()
        {
            const int NonExistingId = 999999;

            // Arrange
            var client = _factory.CreateClient();

            // Act
            var cmd = new[] { new PatchCommand("replace", "/email", "new@somemail.com") };
            var jsonString = JsonSerializer.Serialize(cmd, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json-patch+json");
            var response = await client.PatchAsync($"/api/randomusers/{NonExistingId}", httpContent);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async void Patch_UnsafeButIdempotent()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var Existing = await FirstExistingAsync();
            var oldEmail = Existing.Email;

            var newEmail = $"{Guid.NewGuid()}@somemail.com";
            var cmd = new[] { new PatchCommand("replace", "/email", newEmail) };
            var jsonString = JsonSerializer.Serialize(cmd, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json-patch+json");
            var response = await client.PatchAsync($"/api/randomusers/{Existing.Id}", httpContent);
            var responseAgain = await client.PatchAsync($"/api/randomusers/{Existing.Id}", httpContent);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            Assert.Equal(HttpStatusCode.NoContent, responseAgain.StatusCode);

            var newValue = await GetExistingAsync(Existing.Id);
            Assert.NotEqual(newValue.Email, oldEmail);
            Assert.Equal(newValue.Email, newEmail);
        }

        [Fact]
        public async void Delete_ExistingId_ReturnsNoContent()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var ExistingId = await FirstExistingIdAsync();
            var response = await client.DeleteAsync($"/api/randomusers/{ExistingId}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async void Delete_NoExistingId_ReturnsNotFound()
        {
            //Arrange
            var client = _factory.CreateClient();

            // Act
            const int NonExistingId = 999999999;
            var response = await client.DeleteAsync($"/api/randomusers/{NonExistingId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async void Delete_UnsafeButIdempotent()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var ExistingId = await FirstExistingIdAsync();
            var response = await client.DeleteAsync($"/api/randomusers/{ExistingId}");
            var responseAfterDelete = await client.GetAsync($"/api/randomusers/{ExistingId}");
            var responseDeleteAgain = await client.DeleteAsync($"/api/randomusers/{ExistingId}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, responseAfterDelete.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, responseDeleteAgain.StatusCode);
        }

        private async Task<int> FirstExistingIdAsync()
        {
            var existing = await FirstExistingAsync();
            return existing != null ? existing.Id : 0;
        }

        private async Task<RandomUserDto> FirstExistingAsync()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/api/randomusers?results=1");
            if (!response.IsSuccessStatusCode) {
                throw new ApplicationException("Get error on reading existing items");
            }
            var content = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrWhiteSpace(content));

            var users = JsonSerializer.Deserialize<IList<RandomUserDto>>(content, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            if (!users.Any()) {
                throw new ApplicationException("Integration test ran off all items in list, re-initiate test-database again");
            } else {
                return users.First();
            }
        }

        private async Task<RandomUserDto> GetExistingAsync(int id)
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync($"/api/randomusers/{id}");
            if (!response.IsSuccessStatusCode) {
                throw new ApplicationException("Get error on reading existing items");
            }
            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(content)) {
                return null;
            }

            return JsonSerializer.Deserialize<RandomUserDto>(content, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }
    }

    internal class PatchCommand
    {
        public string Op { get; set; }
        public string Path { get; set; }
        public string Value { get; set; }

        public PatchCommand(string op, string path, string value = null)
        {
            Op = op;
            Path = path;
            Value = value;
        }
    }
}
