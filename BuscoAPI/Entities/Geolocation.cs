namespace BuscoAPI.Entities
{
    public class Geolocation
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Geolocation(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
