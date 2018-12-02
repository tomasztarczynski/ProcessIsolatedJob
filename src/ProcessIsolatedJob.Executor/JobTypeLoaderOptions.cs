namespace ProcessIsolatedJob.Executor
{
    public class JobTypeLoaderOptions
    {
        public JobTypeLoaderOptions(string jobAssemblyPath)
        {
            JobAssemblyPath = jobAssemblyPath;
        }

        public string JobAssemblyPath { get; }
    }
}