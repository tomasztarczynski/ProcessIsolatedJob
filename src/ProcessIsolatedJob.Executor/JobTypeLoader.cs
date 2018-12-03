using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;

namespace ProcessIsolatedJob.Executor
{
    internal class JobTypeLoader
    {
        private readonly ILogger<JobTypeLoader> _logger;
        private readonly JobTypeLoaderOptions _options;

        public JobTypeLoader(ILogger<JobTypeLoader> logger, JobTypeLoaderOptions options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public Type Load()
        {
            try
            {
                _logger.LogInformation("Loading job type from {JobAssemblyPath}...", _options.JobAssemblyPath);

                var stopwatch = Stopwatch.StartNew();

                var pluginLoader = McMaster.NETCore.Plugins.PluginLoader.CreateFromAssemblyFile(
                    _options.JobAssemblyPath,
                    sharedTypes: new[]
                    {
                        typeof(IProcessIsolatedJob),
                        typeof(ILogger),
                        typeof(IConfiguration)
                    });

                var jobTypeAssembly = pluginLoader.LoadDefaultAssembly();

                var jobTypes = jobTypeAssembly.GetTypes()
                    .Where(type => typeof(IProcessIsolatedJob).IsAssignableFrom(type)
                        && !type.IsAbstract
                        && type.IsClass)
                    .ToList();

                if (jobTypes.Count != 1)
                {
                    throw new InvalidOperationException(
                        $"{_options.JobAssemblyPath} should contains exactly one job type that is not abstract class which implements {typeof(IProcessIsolatedJob).Name} interface");
                }

                var jobType = jobTypes.Single();

                if (jobType.GetConstructors().Count() != 1)
                {
                    throw new InvalidOperationException("Job type should have exactly one constructor");
                }

                if (jobType.GetConstructor(new[] { typeof(ILogger), typeof(IConfiguration) }) == null)
                {
                    throw new InvalidOperationException(
                        $"Job type should have ({typeof(ILogger).Name}, {typeof(IConfiguration).Name}) constructor");
                }

                _logger.LogInformation(
                    "{JobTypeName} job type has been loaded successfully in {ElapsedMs} milliseconds",
                    jobType.Name,
                    stopwatch.ElapsedMilliseconds);

                return jobType;
            }
            catch
            {
                _logger.LogError("Job type loading has failed");
                throw;
            }
        }
    }
}