using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ProcessIsolatedJob.ExampleJob
{
    public class UnsafeJob : IProcessIsolatedJob
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public UnsafeJob(ILogger logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public Task ExecuteAsync()
        {
            _logger.LogInformation(_configuration["Message"]);
            return Task.CompletedTask;
        }
    }
}