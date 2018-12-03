using System;

namespace ProcessIsolatedJob.Executor
{
    public class JobTypeLoaderOptions
    {
        public JobTypeLoaderOptions(string jobAssemblyPath)
        {
            if (string.IsNullOrWhiteSpace(jobAssemblyPath))
            {
                throw new ArgumentException("Value cannot be null or empty", nameof(jobAssemblyPath));
            }

            JobAssemblyPath = jobAssemblyPath.Trim();
        }

        public string JobAssemblyPath { get; }
    }
}