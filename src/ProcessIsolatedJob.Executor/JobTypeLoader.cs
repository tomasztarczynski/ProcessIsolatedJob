using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;

namespace ProcessIsolatedJob.Executor
{
    internal class JobTypeLoader
    {
        private readonly ILogger<JobTypeLoader> _logger;
        private readonly JobTypeLoaderOptions _options;

        public JobTypeLoader(ILogger<JobTypeLoader> logger, JobTypeLoaderOptions options)
        {
            _logger = logger;
            _options = options;
        }

        public Type Load()
        {
            var jobAssembly = Assembly.LoadFrom(_options.JobAssemblyPath);

            var jobTypes = jobAssembly.DefinedTypes
                .Where(type => typeof(IProcessIsolatedJob).IsAssignableFrom(type))
                .ToList();

            if (jobTypes.Count != 1)
            {
                throw new InvalidOperationException("Assembly don't have any job");
            }

            var jobType = jobTypes.Single();

            if (jobType.DeclaredConstructors.Count() != 1)
            {
                throw new InvalidOperationException("Type has too many constructors");
            }

            if (jobType.GetConstructor(Type.EmptyTypes) == null)
            {
                throw new InvalidOperationException("Type missing empty constructor");
            }

            return jobType;
        }
    }
}