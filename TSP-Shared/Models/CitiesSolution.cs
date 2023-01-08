using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TSP_Shared.Algorithms;

namespace TSP_Shared.Models
{
    [Serializable]
    public class CitiesSolution
    {
        public List<City> Cities { get; set; }

        public int Length { get => Cities.Count; }

        public CitiesSolution(List<City> cities)
        {
            Cities = cities;
        }

        public override string ToString()
        {
            var concated = "";

            foreach (var cities in Cities)
            {
                if (cities == null)
                {
                    concated += $"  ";
                }
                else
                {
                    concated += $"{cities.Id} ";
                }

            }

            return concated;
        }
        
        public City GetVertexAt(int index)
        {
            return Cities[index];
        }
        
        private bool ContainsVertexWithNumber(int id)
        {
            return Cities.Any(city => city != null && city.Id == id);
        }

        public bool ContainsVertex(City city)
        {
            return ContainsVertexWithNumber(city.Id);
        }

        public void SetNextEmptyVertex(City city)
        {
            for (int i = 0; i < Cities.Count; i++)
            {
                if (Cities[i] == null)
                {
                    Cities[i] = city;
                    return;
                }
            }
        }

        public double CalculateTotalDistance()
        {
            double distance = 0;

            for(int i=0; i < Cities.Count; i++)
            {
                var current = Cities[i];
                var next = Cities[(i + 1) % Cities.Count];

                distance += current.CountDistance(next);
            }

            return distance;
        }

        public CitiesSolution GetShuffledCopy()
        {
            var citiesCopy = new List<City>(Cities);
            citiesCopy.Shuffle();

            return new CitiesSolution(citiesCopy);
        }

        public CitiesSolution GetCopy()
        {
            var vertexesCopy = new List<City>(Cities);

            return new CitiesSolution(vertexesCopy);
        }
    }
}
