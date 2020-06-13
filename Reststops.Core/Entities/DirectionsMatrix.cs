namespace Reststops.Core.Entities
{
    public class DirectionsMatrix
    {
        public string Code { get; set; }
        public double[][] Distances { get; set; }
        public double[][] Durations { get; set; }
    }
}
