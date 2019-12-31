using System;
using System.Linq;
using Xunit;
using CeloPracticalChallenge.API.Models;
using CeloPracticalChallenge.API.Repositories;

namespace CeloPracticalChallenge.API.Tests
{
    public class RandomUserRepositoryTests
    {
        [Fact]
        public void List_NullParameters_ThrowExceptionAsync()
        {
            // Arrange
            var options = Arrange.NewMemoryDBWithTestData();

            using (var context = new CeloPracticalChallengeDBContext(options)) {
                var userRepository = new RandomUserRepository(context);
                // Assert
                Assert.ThrowsAsync<ArgumentNullException>(async () => {
                    // Act
                    var users = await userRepository.ListAsync(null);
                });
            }
        }

        [Fact]
        public async void List_ResultsIsThree_ReturnsThreeUsersAsync()
        {
            // Arrange
            var options = Arrange.NewMemoryDBWithTestData();

            using (var context = new CeloPracticalChallengeDBContext(options)) {
                var userRepository = new RandomUserRepository(context);

                // Act
                var users = await userRepository.ListAsync(
                    new ResourceParameters.RandomUsersResourceParameters {
                        Results = 3
                    });

                // Assert
                Assert.Equal(3, users.Count());
            }
        }

        [Fact]
        public async void List_FirstIsJessica_ReturnsJessicaAsync()
        {
            // Arrange
            var options = Arrange.NewMemoryDBWithTestData();

            using (var context = new CeloPracticalChallengeDBContext(options)) {
                var userRepository = new RandomUserRepository(context);

                // Act
                var users = await userRepository.ListAsync(
                    new ResourceParameters.RandomUsersResourceParameters {
                        FirstName = "Jessica"
                    });

                // Assert
                Assert.All(users, u => Assert.Equal("Jessica", u.FirstName));
            }
        }

        [Fact]
        public async void List_LastIsChen_ReturnsChenAsync()
        {
            // Arrange
            var options = Arrange.NewMemoryDBWithTestData();

            using (var context = new CeloPracticalChallengeDBContext(options)) {
                var userRepository = new RandomUserRepository(context);

                // Act
                var users = await userRepository.ListAsync(
                    new ResourceParameters.RandomUsersResourceParameters {
                        LastName = "Chen"
                    });

                // Assert
                Assert.All(users, u => Assert.Equal("Chen", u.LastName));
            }
        }

        [Fact]
        public async void Get_NonExistingId_ReturnsNull()
        {
            // Arrange
            var options = Arrange.NewMemoryDBWithTestData();

            using (var context = new CeloPracticalChallengeDBContext(options)) {
                var userRepository = new RandomUserRepository(context);

                // Act
                var user = await userRepository.GetAsync(999);

                // Assert
                Assert.Null(user);
            }
        }

        [Fact]
        public async void Get_IdTwo_ReturnsIdTwo()
        {
            // Arrange
            var options = Arrange.NewMemoryDBWithTestData();

            using (var context = new CeloPracticalChallengeDBContext(options)) {
                var userRepository = new RandomUserRepository(context);

                // Act
                var user = await userRepository.GetAsync(2);

                // Assert
                Assert.Equal(2, user.Id);
            }
        }

        [Fact]
        public async void Modify_IdTwo_ReturnsTrue()
        {
            // Arrange
            var options = Arrange.NewMemoryDBWithTestData();

            using (var context = new CeloPracticalChallengeDBContext(options)) {
                var userRepository = new RandomUserRepository(context);

                // Act
                var user = new Entities.RandomUser {
                    Id = 2,
                    Email = "new.user@example.com",
                    Title = "Mr",
                    FirstName = "New",
                    LastName = "User",
                    DateOfBirth = new DateTime(1990, 1, 1),
                    PhoneNumber = "(204)-997-2604",
                    ThrumbnailURL = "https://randomuser.me/api/portraits/thumb/men/61.jpg",
                    LargeImageURL = "https://randomuser.me/api/portraits/portraits/men/61.jpg",
                };

                bool ret = await userRepository.ModifyAsync(user);

                // Assert
                Assert.True(ret);
            }
        }

        [Fact]
        public async void Modify_NoExistingId_ReturnsFalse()
        {
            // Arrange
            var options = Arrange.NewMemoryDBWithTestData();

            using (var context = new CeloPracticalChallengeDBContext(options)) {
                var userRepository = new RandomUserRepository(context);

                // Act
                var user = new Entities.RandomUser {
                    Id = -2,
                    Email = "new.user@example.com",
                    Title = "Mr",
                    FirstName = "New",
                    LastName = "User",
                    DateOfBirth = new DateTime(1990, 1, 1),
                    PhoneNumber = "(204)-997-2604",
                    ThrumbnailURL = "https://randomuser.me/api/portraits/thumb/men/61.jpg",
                    LargeImageURL = "https://randomuser.me/api/portraits/portraits/men/61.jpg",
                };

                bool ret = await userRepository.ModifyAsync(user);

                // Assert
                Assert.False(ret);
            }
        }

        [Fact]
        public async void Delete_IdTwo_ReturnsTrue()
        {
            // Arrange
            var options = Arrange.NewMemoryDBWithTestData();

            using (var context = new CeloPracticalChallengeDBContext(options)) {
                var userRepository = new RandomUserRepository(context);

                // Act
                bool ret = await userRepository.DeleteAsync(2);

                // Assert
                Assert.True(ret);
            }
        }

        [Fact]
        public async void Delete_NoExistingId_ReturnsFalse()
        {
            // Arrange
            var options = Arrange.NewMemoryDBWithTestData();

            using (var context = new CeloPracticalChallengeDBContext(options)) {
                var userRepository = new RandomUserRepository(context);

                // Act
                bool ret = await userRepository.DeleteAsync(999);

                // Assert
                Assert.False(ret);
            }
        }
    }
}
