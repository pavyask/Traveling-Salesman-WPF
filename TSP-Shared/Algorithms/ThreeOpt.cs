using System;
using System.Collections.Generic;
using System.Text;
using TSP_Shared.Models;

namespace TSP_Shared.Algorithms
{
    public class ThreeOpt
    {
        public static CitiesSolution Process(CitiesSolution solution)
        {
            var random = new Random();
            var randomPosition = random.Next(0, solution.Length - 3);

            var city1 = solution.GetVertexAt(randomPosition);
            var city2 = solution.GetVertexAt(randomPosition + 1);
            var city3 = solution.GetVertexAt(randomPosition + 2);

            var newSolution = new CitiesSolution(solution.Cities);

            newSolution.Cities[randomPosition] = city2;
            newSolution.Cities[randomPosition + 1] = city3;
            newSolution.Cities[randomPosition + 2] = city1;

            return newSolution;
        }
    }
}
