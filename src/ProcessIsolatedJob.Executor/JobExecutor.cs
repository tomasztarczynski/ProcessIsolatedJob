using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ProcessIsolatedJob.Executor
{
    internal class JobExecutor
    {
        private readonly ILogger<JobExecutor> _logger;
        private readonly JobTypeLoader _jobTypeLoader;
        private readonly JobConfigurationReader _jobConfigurationReader;
        private readonly JobActivator _jobActivator;

        public JobExecutor(
            ILogger<JobExecutor> logger,
            JobTypeLoader jobTypeLoader,
            JobConfigurationReader jobConfigurationReader,
            JobActivator jobActivator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _jobTypeLoader = jobTypeLoader ?? throw new ArgumentNullException(nameof(jobTypeLoader));
            _jobConfigurationReader = jobConfigurationReader ?? throw new ArgumentNullException(nameof(jobConfigurationReader));
            _jobActivator = jobActivator ?? throw new ArgumentNullException(nameof(jobActivator));
        }

        public async Task ExecuteAsync()
        {
            var jobType = _jobTypeLoader.Load();
            var jobConfiguration = _jobConfigurationReader.Read();
            var job = _jobActivator.CreateInstance(jobType, jobConfiguration);

            try
            {
                _logger.LogInformation("Executing {JobTypeName} job...", jobType.Name);

                var stopwatch = Stopwatch.StartNew();

                await job.ExecuteAsync();

                _logger.LogInformation(
                    "{JobTypeName} job has executed successfully in {ElapsedMs} milliseconds",
                    jobType.Name,
                    stopwatch.ElapsedMilliseconds);
            }
            catch
            {
                _logger.LogError("{JobTypeName} job execution has failed", jobType.Name);
                throw;
            }
        }
    }
}