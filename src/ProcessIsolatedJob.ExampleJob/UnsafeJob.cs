using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using RestSharp;
using System;

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

            var request = new RestRequest("json/utc/now", dataFormat: DataFormat.Json);
            var client = new RestClient("http://worldclockapi.com/api");
            var response = client.Execute<dynamic>(request);
            _logger.LogInformation($"Current UTC time: {{0}}", (string)response.Data["currentDateTime"]);
            return Task.CompletedTask;
        }
    }
}