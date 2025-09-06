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
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var cutoff = DateTime.UtcNow.AddMinutes(-5);
                var unconfirmedUsers = dbContext.Users
                    .Where(u => !u.EmailConfirmed && u.CreatedAt < cutoff)
                    .ToList();

                foreach (var user in unconfirmedUsers)
                {
                    await userManager.DeleteAsync(user);
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // chạy mỗi phút
            }
        }
    }

}
