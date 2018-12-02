using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace ProcessIsolatedJob.Executor
{
    internal class JobActivator
    {
        private readonly ILogger<JobActivator> _logger;
        private readonly ILoggerFactory _loggerFactory;

        public JobActivator(ILogger<JobActivator> logger, ILoggerFactory loggerFactory)
        {
            _logger = logger;
            _loggerFactory = loggerFactory;
        }

        public IProcessIsolatedJob CreateInstance(Type jobType, IConfiguration jobConfiguration)
        {
            return (IProcessIsolatedJob)Activator.CreateInstance(
                jobType,
                _loggerFactory.CreateLogger(jobType),
                jobConfiguration);
        }
    }
}