﻿using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext>(this IHost host,
                                                      Action<TContext, IServiceProvider> seeder) where TContext : DbContext
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<TContext>>();
                var context = services.GetService<TContext>();

                try
                {
                    logger.LogInformation("Migrating database associated with context {DbContextname}", typeof(TContext).Name);

                    var retry = Policy.Handle<SqlException>()
                        .WaitAndRetry(retryCount: 5,
                        sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        onRetry: (exception, retryCount, context) =>
                        {
                            logger.LogError($"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey}, due to: {exception}.");
                        });

                    retry.Execute(() => InvokeSeeder(seeder, context, services));

                    logger.LogInformation("Migrated database associated with context {DbContextname}", typeof(TContext).Name);

                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the database used on context");
                }
            }

            return host;
        }

        private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder,
                                                   TContext context,
                                                   IServiceProvider services)
                                                   where TContext : DbContext
        {
            context.Database.Migrate();
            seeder(context, services);
        }
    }
}
