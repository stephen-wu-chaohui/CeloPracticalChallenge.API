using System;
using CeloPracticalChallenge.API.Controllers;
using CeloPracticalChallenge.API.Models;
using CeloPracticalChallenge.API.Profiles;
using CeloPracticalChallenge.API.Repositories;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace CeloPracticalChallenge.API.Tests
{
    public static class Arrange
    {
        public static DbContextOptions<CeloPracticalChallengeDBContext> NewMemoryDBWithTestData()
        {
            var options = new DbContextOptionsBuilder<CeloPracticalChallengeDBContext>()
                .UseInMemoryDatabase($"DBForTesting{Guid.NewGuid()}")
                .Options;

            using (var context = new CeloPracticalChallengeDBContext(options)) {
                context.RandomUser.Add(new Entities.RandomUser {
                    Id = 1,
                    Email = "jessica.green@example.com",
                    Title = "Ms",
                    FirstName = "Jessica",
                    LastName = "Green",
                    DateOfBirth = new DateTime(1975, 2, 1),
                    PhoneNumber = "(768)-085-8755",
                    ThrumbnailURL = "https://randomuser.me/api/portraits/thumb/women/35.jpg",
                    LargeImageURL = "https://randomuser.me/api/portraits/women/35.jpg",
                });
                context.RandomUser.Add(new Entities.RandomUser {
                    Id = 2,
                    Email = "nathaniel.harris@example.com",
                    Title = "Mr",
                    FirstName = "Nathaniel",
                    LastName = "Harris",
                    DateOfBirth = new DateTime(1990, 1, 1),
                    PhoneNumber = "(204)-997-2604",
                    ThrumbnailURL = "https://randomuser.me/api/portraits/thumb/men/61.jpg",
                    LargeImageURL = "https://randomuser.me/api/portraits/portraits/men/61.jpg",
                });
                context.RandomUser.Add(new Entities.RandomUser {
                    Id = 3,
                    Email = "jake.morris@example.com",
                    Title = "Mr",
                    FirstName = "Jake",
                    LastName = "Morris",
                    DateOfBirth = new DateTime(1981, 1, 1),
                    PhoneNumber = "(204)-997-2604",
                    ThrumbnailURL = "https://randomuser.me/api/portraits/thumb/men/64.jpg",
                    LargeImageURL = "https://randomuser.me/api/portraits/portraits/men/64.jpg",
                });
                context.RandomUser.Add(new Entities.RandomUser {
                    Id = 4,
                    Email = "evie.chen@example.com",
                    Title = "Mrs",
                    FirstName = "Evie",
                    LastName = "Chen",
                    DateOfBirth = new DateTime(1980, 1, 1),
                    PhoneNumber = "(204)-997-2604",
                    ThrumbnailURL = "https://randomuser.me/api/portraits/thumb/women/47.jpg",
                    LargeImageURL = "https://randomuser.me/api/portraits/portraits/women/47.jpg",
                });
                context.RandomUser.Add(new Entities.RandomUser {
                    Id = 5,
                    Email = "jessica.kumar@example.com",
                    Title = "Mrs",
                    FirstName = "Jessica",
                    LastName = "Kumar",
                    DateOfBirth = new DateTime(1948, 1, 1),
                    PhoneNumber = "(204)-997-2604",
                    ThrumbnailURL = "https://randomuser.me/api/portraits/thumb/women/35.jpg",
                    LargeImageURL = "https://randomuser.me/api/portraits/portraits/women/35.jpg",
                });

                context.SaveChanges();
            }

            return options;
        }

        public static IRandomUserRepository NewRepositoryForTesting()
        {
            var context = new CeloPracticalChallengeDBContext(NewMemoryDBWithTestData());
            return new RandomUserRepository(context);
        }

        public static RandomUsersController NewControllerForTesting()
        {
            var newRepository = NewRepositoryForTesting();
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile(new RandomUsersProfile());
            });
            IMapper mapper = config.CreateMapper();

            return new RandomUsersController(newRepository);
        }

    }
}
