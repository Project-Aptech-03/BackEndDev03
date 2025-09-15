using Microsoft.AspNetCore.Identity;
using ProjectDemoWebApi.Data;
using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Validation
{
    public class UnconfirmedUserCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public UnconfirmedUserCleanupService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Users>>();
                
                // For now, we'll just check every hour instead of using CreatedAt
                // This is a safer approach without the CreatedAt field
                
                var unconfirmedUsers = userManager.Users
                    .Where(u => !u.EmailConfirmed)
                    .Take(50) // Limit for performance
                    .ToList();

                // Log the count for monitoring
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<UnconfirmedUserCleanupService>>();
                if (unconfirmedUsers.Count > 0)
                {
                    logger.LogInformation($"Found {unconfirmedUsers.Count} unconfirmed users");
                }

                // We'll be conservative and not auto-delete without CreatedAt timestamp
                // This prevents accidentally deleting legitimate users
                
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken); // Run every hour
            }
        }
    }
}
