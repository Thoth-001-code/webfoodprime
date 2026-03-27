using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using webfoodprime.Data;
using webfoodprime.Models;

namespace webfoodprime.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.UseEnvironment("Test");

            builder.ConfigureServices(services =>
            {
                // Remove existing AppDbContext registration
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // Add InMemory DB
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });

                // Add test authentication
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                    options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                }).AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });

                // Build the service provider
                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<AppDbContext>();

                    // Ensure database is created
                    db.Database.EnsureCreated();

                    // Seed basic data
                    if (!db.Foods.Any())
                    {
                        db.Foods.Add(new Food { FoodName = "Burger", Price = 50000M });
                        db.Foods.Add(new Food { FoodName = "Pizza", Price = 80000M });
                        db.SaveChanges();
                    }

                    if (!db.Users.Any())
                    {
                        db.Users.Add(new ApplicationUser { Id = "staff-1", UserName = "staff1", Email = "staff1@example.com" });
                        db.Users.Add(new ApplicationUser { Id = "customer-1", UserName = "cust1", Email = "cust1@example.com" });
                        db.SaveChanges();
                    }
                }
            });

            return base.CreateHost(builder);
        }
    }
}
