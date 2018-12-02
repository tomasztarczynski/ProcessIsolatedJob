using McMaster.NETCore.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

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
            var pluginLoader = PluginLoader.CreateFromAssemblyFile(
                    _options.JobAssemblyPath,
                    sharedTypes: new[]
                    {
                        typeof(IProcessIsolatedJob),
                        typeof(ILogger),
                        typeof(IConfiguration)
                    });

            var jobAssembly = pluginLoader.LoadDefaultAssembly();

            var jobTypes = jobAssembly.GetTypes()
                .Where(type => typeof(IProcessIsolatedJob).IsAssignableFrom(type)
                    && !type.IsAbstract
                    && type.IsClass)
                .ToList();

            if (jobTypes.Count != 1)
            {
                throw new InvalidOperationException("Assembly don't have any job");
            }

            var jobType = jobTypes.Single();

            if (jobType.GetConstructors().Count() != 1)
            {
                throw new InvalidOperationException("Type has too many constructors");
            }

            if (jobType.GetConstructor(new[] { typeof(ILogger), typeof(IConfiguration) }) == null)
            {
                throw new InvalidOperationException($"Type missing {typeof(ILogger).Name}, {typeof(ILogger).Name} constructor");
            }

            return jobType;
        }
    }
}