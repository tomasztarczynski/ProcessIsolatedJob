using System;

namespace ProcessIsolatedJob.Executor
{
    internal class JobConfigurationReaderOptions
    {
        public JobConfigurationReaderOptions(
            string jobJsonConfigurationPath,
            string jobConfigurationSectionKey)
        {
            if (string.IsNullOrWhiteSpace(jobJsonConfigurationPath))
            {
                throw new ArgumentException("Value cannot be null or empty", nameof(jobJsonConfigurationPath));
            }

            JobJsonConfigurationPath = jobJsonConfigurationPath.Trim();
            JobConfigurationSectionKey = string.IsNullOrWhiteSpace(jobConfigurationSectionKey)
                ? null : jobConfigurationSectionKey.Trim();
        }

        public string JobJsonConfigurationPath { get; }

        public string JobConfigurationSectionKey { get; }
    }
}