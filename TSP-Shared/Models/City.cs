using System;

namespace TSP_Shared.Models
{
    [Serializable]
    public class City
    {
        public int Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

        public double CountDistance(City city)
        {
            return Math.Sqrt(Math.Pow(X - city.X, 2) + Math.Pow(Y - city.Y, 2));
        }
    }
}