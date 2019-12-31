using CeloPracticalChallenge.API.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace CeloPracticalChallenge.API.Tests
{
    public class TestWebFactory : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services => {

                services.RemoveDbContext<CeloPracticalChallengeDBContext>();

                // Create a new service provider.
                var serviceProvider = new ServiceCollection()
                    .AddEntityFrameworkInMemoryDatabase()
                    .BuildServiceProvider();

                // Add a database context (AppDbContext) using an in-memory database for testing.
                services.AddDbContext<CeloPracticalChallengeDBContext>(options => {
                    options.UseInMemoryDatabase("InMemoryAppDb");
                    options.UseInternalServiceProvider(serviceProvider);
                });

                // Build the service provider.
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database contexts
                using (var scope = sp.CreateScope()) {
                    var scopedServices = scope.ServiceProvider;
                    var appDBContext = scopedServices.GetRequiredService<CeloPracticalChallengeDBContext>();

                    // Ensure the database is created.
                    appDBContext.Database.EnsureCreated();
                }
            });
        }
    }

    public static class TestWebFactoryHelper
    {
        public static void RemoveDbContext<TDBContext>(this IServiceCollection services) where TDBContext: DbContext
        {
            var serviceDescriptor = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(DbContextOptions<TDBContext>));
            services.Remove(serviceDescriptor);
            serviceDescriptor = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(TDBContext));
            services.Remove(serviceDescriptor);
        }
    }
}
