using System.Threading.Tasks;
using Reststops.Domain.Entities;

namespace Reststops.Core.Interfaces.Services
{
    public interface IGeocodingService
    {
        public Task<ForwardGeocoding> GetForwardGeocoding(string text);
    }
}
