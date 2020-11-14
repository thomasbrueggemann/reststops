namespace Reststops.Core.Entities
{
    public record DirectionsMatrix
    {
        public string Code { get; init; }
        public double[][] Distances { get; init; }
        public double[][] Durations { get; init; }
    }
}
