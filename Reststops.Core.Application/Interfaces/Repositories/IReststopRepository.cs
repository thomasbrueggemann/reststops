using System.Threading.Tasks;
using Reststops.Domain.Entities;

namespace Reststops.Core.Interfaces.Repositories
{
    public interface IReststopRepository
    {
        Task Insert(Reststop reststop);
    }
}
