using System.Collections.Generic;

namespace Reststops.Presentation.API.Models
{
    public class ReststopsModel
    {
        public IEnumerable<ReststopModel> Reststops { get; set; }
        public string Route { get; set; }
        public string ReststopsRoute { get; set; }
    }
}
