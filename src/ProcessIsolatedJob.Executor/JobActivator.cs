using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace ProcessIsolatedJob.Executor
{
    internal class JobActivator
    {
        private readonly ILogger<JobActivator> _logger;
        private readonly ILoggerFactory _loggerFactory;

        public JobActivator(ILogger<JobActivator> logger, ILoggerFactory loggerFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public IProcessIsolatedJob CreateInstance(Type jobType, IConfiguration jobConfiguration)
        {
            try
            {
                _logger.LogInformation("Creating {JobTypeName} job instance...", jobType.Name);

                var stopwatch = Stopwatch.StartNew();

                var jobInstance = (IProcessIsolatedJob)Activator.CreateInstance(
                    jobType,
                    _loggerFactory.CreateLogger(jobType),
                    jobConfiguration);

                _logger.LogInformation(
                    "{JobTypeName} job instance has been created successfully in {ElapsedMs} milliseconds",
                    jobType.Name,
                    stopwatch.ElapsedMilliseconds);

                return jobInstance;
            }
            catch
            {
                _logger.LogInformation("{JobTypeName} job instance creation has failed", jobType.Name);
                throw;
            }
        }
    }
}