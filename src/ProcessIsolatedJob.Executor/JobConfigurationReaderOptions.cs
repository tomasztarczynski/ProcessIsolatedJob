namespace ProcessIsolatedJob.Executor
{
    internal class JobConfigurationReaderOptions
    {
        public JobConfigurationReaderOptions(
            string jobJsonConfigurationPath,
            string jobConfigurationSectionKey)
        {
            JobJsonConfigurationPath = jobJsonConfigurationPath;
            JobConfigurationSectionKey = jobConfigurationSectionKey;
        }

        public string JobJsonConfigurationPath { get; }

        public string JobConfigurationSectionKey { get; }
    }
}