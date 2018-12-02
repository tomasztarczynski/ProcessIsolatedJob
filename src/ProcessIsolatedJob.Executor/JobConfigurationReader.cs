using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ProcessIsolatedJob.Executor
{
    internal class JobConfigurationReader
    {
        private readonly ILogger<JobConfigurationReader> _logger;
        private readonly JobConfigurationReaderOptions _options;

        public JobConfigurationReader(ILogger<JobConfigurationReader> logger, JobConfigurationReaderOptions options)
        {
            _logger = logger;
            _options = options;
        }

        public IConfiguration Read()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(_options.JobJsonConfigurationPath)
                .Build();

            if (string.IsNullOrEmpty(_options.JobConfigurationSectionKey))
            {
                return configuration;
            }
            else
            {
                return configuration.GetSection(_options.JobConfigurationSectionKey);
            }
        }
    }
}