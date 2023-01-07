using System.Collections.Generic;
using System.Globalization;
using System.IO;
using TSP_Shared.Models;

namespace TSP_WPF.Helpers
{
    public static class TspFileLoader
    {
        public static List<City> CreateCitiesListFromFile(string filePath)
        {
            List<City> cities = new List<City>();
            string[] lines = File.ReadAllLines(filePath);

            for (int i = 5; !lines[i].Equals("EOF"); i++)
            {
                string[] split = lines[i].Split(" ");
                cities.Add(new City
                {
                    Id = int.Parse(split[0]),
                    X = double.Parse(split[1], CultureInfo.InvariantCulture),
                    Y = double.Parse(split[2], CultureInfo.InvariantCulture)
                });
            }

            return cities;
        }
    }
}
