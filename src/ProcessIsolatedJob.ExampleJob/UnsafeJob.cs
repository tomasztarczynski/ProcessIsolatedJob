using System.Threading.Tasks;

namespace ProcessIsolatedJob.ExampleJob
{
    public class UnsafeJob : IProcessIsolatedJob
    {
        public Task ExecuteAsync()
        {
            return Task.CompletedTask;
        }
    }
}