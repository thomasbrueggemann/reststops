namespace Reststops.Presentation.API.Models
{
    public record PlaceModel
    {
        public string Name { get; init; }
        public double Latitude { get; init; }
        public double Longitude { get; init; }
    }
}
