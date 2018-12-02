using Microsoft.Extensions.Logging;
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
            _logger = logger;
            _jobTypeLoader = jobTypeLoader;
            _jobConfigurationReader = jobConfigurationReader;
            _jobActivator = jobActivator;
        }

        public async Task ExecuteAsync()
        {
            var jobType = _jobTypeLoader.Load();
            var jobConfiguration = _jobConfigurationReader.Read();
            var job = _jobActivator.CreateInstance(jobType, jobConfiguration);
            await job.ExecuteAsync();
        }
    }
}