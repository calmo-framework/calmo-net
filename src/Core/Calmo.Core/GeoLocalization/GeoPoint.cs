using Calmo.Core.ExceptionHandling;

namespace System
{
    public class GeoPoint
    {
        public GeoPoint() { }

        public GeoPoint(decimal latitude, decimal longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        public static bool IsValid(decimal? latitude, decimal? longitude)
        {
            if (!latitude.HasValue || !longitude.HasValue)
                return false;

            if (latitude.Value == 0m || longitude.Value == 0m)
                return false;

            return (latitude.Value >= -90m && latitude.Value <= 90) && (longitude.Value >= -180 && longitude.Value <= 180);
        }

        public static double Distance(GeoPoint pointA, GeoPoint pointB)
        {
            Throw.IfArgumentNull(pointA, nameof(pointA));
            Throw.IfArgumentNull(pointB, nameof(pointB));

            const int earthRadius = 6371;

            var latitude = ((double)pointB.Latitude - (double)pointA.Latitude) * Math.PI / 180;
            var longitude = ((double)pointB.Longitude - (double)pointA.Longitude) * Math.PI / 180;

            var a = Math.Sin(latitude / 2) * Math.Sin(latitude / 2) + Math.Cos((double)pointA.Latitude * Math.PI / 180) * Math.Cos((double)pointB.Latitude * Math.PI / 180) * Math.Sin(longitude / 2) * Math.Sin(longitude / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return earthRadius * c;
        }
    }
}