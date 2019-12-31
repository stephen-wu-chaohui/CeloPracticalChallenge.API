using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using CeloPracticalChallenge.API.Entities;
using CeloPracticalChallenge.API.ExternalDtos;

namespace CeloPracticalChallenge.API.Models
{
    public class CeloPracticalChallengeDBContext : DbContext
    {
        private const string initJson = "./App_Data/InitRandomUsers.json";

        public CeloPracticalChallengeDBContext(DbContextOptions<CeloPracticalChallengeDBContext> options)
            : base(options)
        {
        }

        public DbSet<RandomUser> RandomUser { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            string content = File.ReadAllText(initJson);
            if (!string.IsNullOrWhiteSpace(content)) {
                var users = JsonSerializer.Deserialize<IList<RandomUserInput>>(content, 
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                if (users != null || users.Count > 0) {
                    var config = new MapperConfiguration(cfg => {
                        cfg.CreateMap<ExternalDtos.RandomUserInput, Entities.RandomUser>()
                            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.email))
                            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.name.title))
                            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.name.first))
                            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.name.last))
                            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.dob.date.Date))
                            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.phone))
                            .ForMember(dest => dest.LargeImageURL, opt => opt.MapFrom(src => src.picture.large))
                            .ForMember(dest => dest.ThrumbnailURL, opt => opt.MapFrom(src => src.picture.thumbnail));
                    });
                    IMapper mapper = config.CreateMapper();

                    var randomUsers = users.Select(user => mapper.Map<RandomUser>(user)).ToList();
                    for (int i = 0; i < randomUsers.Count; i ++) {
                        randomUsers[i].Id = i + 1;
                    }
                    modelBuilder.Entity<RandomUser>().HasData(randomUsers);
                }
            }

            base.OnModelCreating(modelBuilder);
        }

    }
}
