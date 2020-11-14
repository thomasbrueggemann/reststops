using System.Collections.Generic;

namespace Reststops.Presentation.API.Models
{
    public record ReststopsModel
    {
        public IEnumerable<ReststopModel> Reststops { get; init; }
        public string Route { get; init; }
        public string ReststopsRoute { get; init; }
    }
}
