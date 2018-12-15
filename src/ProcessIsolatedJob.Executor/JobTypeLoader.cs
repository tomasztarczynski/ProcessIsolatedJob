﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
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
#if NETCOREAPP2_1
                var pluginLoader = McMaster.NETCore.Plugins.PluginLoader.CreateFromAssemblyFile(
                    ToFullPath(_options.JobAssemblyPath),
                    sharedTypes: new[]
                    {
                        typeof(IProcessIsolatedJob),
                        typeof(ILogger),
                        typeof(IConfiguration)
                    });

                var jobTypeAssembly = pluginLoader.LoadDefaultAssembly();
#else
                var jobTypeAssembly = System.Reflection.Assembly.LoadFrom(ToFullPath(_options.JobAssemblyPath));
#endif

                var jobTypes = jobTypeAssembly.GetTypes()
                    .Where(type => typeof(IProcessIsolatedJob).IsAssignableFrom(type)
                        && !type.IsAbstract
                        && type.IsClass)
                    .ToList();

                if (jobTypes.Count != 1)
                {
                    throw new InvalidOperationException(
                        $"{_options.JobAssemblyPath} should contains exactly one job type that is not abstract class which implements {typeof(IProcessIsolatedJob).FullName} interface");
                }

                var jobType = jobTypes.Single();

                if (jobType.GetConstructors().Count() != 1)
                {
                    throw new InvalidOperationException("Job type should have exactly one constructor");
                }

                if (jobType.GetConstructor(new[] { typeof(ILogger), typeof(IConfiguration) }) == null)
                {
                    var constructor = jobType.GetConstructors()[0];
                    var parameters = constructor.GetParameters();

                    if (parameters.Any(p => p.ParameterType.FullName == typeof(ILogger).FullName)
                        && parameters.Any(p => p.ParameterType.FullName == typeof(IConfiguration).FullName))
                    {
                        throw new InvalidOperationException(
                            $"Job type should have correct assembly versions for {typeof(ILogger).FullName} and {typeof(IConfiguration).FullName} types");
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            $"Job type should have ({typeof(ILogger).FullName}, {typeof(IConfiguration).FullName}) constructor");
                    }
                }

                _logger.LogInformation(
                    "{JobTypeFullName} job type has been loaded successfully in {ElapsedMs} milliseconds",
                    jobType.FullName,
                    stopwatch.ElapsedMilliseconds);

                return jobType;
            }
            catch
            {
                _logger.LogError("Job type loading has failed");
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