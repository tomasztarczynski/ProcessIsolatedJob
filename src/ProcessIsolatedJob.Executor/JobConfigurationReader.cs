using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.IO;

namespace ProcessIsolatedJob.Executor
{
    internal class JobConfigurationReader
    {
        private readonly ILogger<JobConfigurationReader> _logger;
        private readonly JobConfigurationReaderOptions _options;

        public JobConfigurationReader(ILogger<JobConfigurationReader> logger, JobConfigurationReaderOptions options)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            _options = options ?? throw new System.ArgumentNullException(nameof(options));
        }

        public IConfiguration Read()
        {
            try
            {
                _logger.LogInformation("Reading job configuration from {JobJsonConfigurationPath}...", _options.JobJsonConfigurationPath);

                var stopwatch = Stopwatch.StartNew();

                var configuration = new ConfigurationBuilder()
                    .AddJsonFile(ToFullPath(_options.JobJsonConfigurationPath))
                    .Build();

                if (_options.JobConfigurationSectionKey == null)
                {
                    _logger.LogInformation(
                        "Job configuration has been read successfully in {ElapsedMs} milliseconds", stopwatch.ElapsedMilliseconds);

                    return configuration;
                }
                else
                {
                    var configurationSection = configuration.GetSection(_options.JobConfigurationSectionKey);

                    _logger.LogInformation(
                        "{JobConfigurationSectionKey} job configuration section has been read successfully in {ElapsedMs} milliseconds",
                        _options.JobConfigurationSectionKey,
                        stopwatch.ElapsedMilliseconds);

                    return configurationSection;
                }
            }
            catch
            {
                _logger.LogError("Job configuration reading has failed");
                throw;
            }
        }

        private string ToFullPath(string path)
        {
            var fileInfo = new FileInfo(path);
            return fileInfo.FullName;
        }
    }
}