using Microsoft.Extensions.Configuration;
using System;

namespace ProcessIsolatedJob.Executor
{
    internal class JobActivator
    {
        public IProcessIsolatedJob CreateInstance(Type jobType, IConfiguration jobConfiguration)
        {
            return null;
        }
    }
}