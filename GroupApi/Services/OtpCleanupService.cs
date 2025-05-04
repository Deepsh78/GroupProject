using GroupApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GroupApi.Services
{
    public class OtpCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public OtpCleanupService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var expiredOtps = await context.OtpRecords
                        .Where(o => o.Expiry < DateTime.UtcNow)
                        .ToListAsync(stoppingToken);

                    context.OtpRecords.RemoveRange(expiredOtps);
                    await context.SaveChangesAsync(stoppingToken);
                }

                // Run every 30 minutes
                await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
            }
        }
    }
}