﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ProcessIsolatedJob.Executor
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            IConfiguration configuration;
            IServiceProvider serviceProvider;
            ILogger logger;

            try
            {
                var stopwatch = Stopwatch.StartNew();
                configuration = BuildConfiguration(args);
                serviceProvider = BuildServiceProvider(configuration);
                logger = serviceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogTrace("Executor has initialized successfully in {InitTimeInMs} milliseconds", stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Log(NLog.LogLevel.Fatal, ex, "Executor initialization has failed");
                throw;
            }

            try
            {
                var jobExecutor = serviceProvider.GetRequiredService<JobExecutor>();
                await jobExecutor.ExecuteAsync();
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Exception has occured during job execution");
                throw;
            }
        }

        private static IConfiguration BuildConfiguration(string[] args)
        {
            return new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();
        }

        private static IServiceProvider BuildServiceProvider(IConfiguration configuration)
        {
            return new ServiceCollection()
                .AddLogging(builder =>
                {
                    builder.SetMinimumLevel(LogLevel.Trace);
                    builder.AddNLog();
                })
                .AddSingleton(GetJobTypeLoaderOptions(configuration))
                .AddSingleton(GetJobConfigurationReaderOptions(configuration))
                .AddTransient<JobTypeLoader>()
                .AddTransient<JobConfigurationReader>()
                .AddTransient<JobActivator>()
                .AddTransient<JobExecutor>()
                .BuildServiceProvider();
        }

        private static JobTypeLoaderOptions GetJobTypeLoaderOptions(IConfiguration configuration)
        {
            return new JobTypeLoaderOptions(
                configuration[nameof(JobTypeLoaderOptions.JobAssemblyPath)]
            );
        }

        private static JobConfigurationReaderOptions GetJobConfigurationReaderOptions(IConfiguration configuration)
        {
            return new JobConfigurationReaderOptions(
                configuration[nameof(JobConfigurationReaderOptions.JobJsonConfigurationPath)],
                configuration[nameof(JobConfigurationReaderOptions.JobConfigurationSectionKey)]
            );
        }
    }
}