using System.Threading.Tasks;

namespace ProcessIsolatedJob
{
    public interface IProcessIsolatedJob
    {
        Task ExecuteAsync();
    }
}
