using System;
using System.Collections.Generic;
using System.Text;
using TSP_Shared.Models;

namespace TSP_Shared.Algorithms
{
    public class PMX
    {
        public static CitiesSolution PMXFirstStep(CitiesSolution solution1, CitiesSolution solution2)
        {
            var random = new Random();
            var resultList = new List<City>(new City[solution1.Length]);
            var resultCycle = new CitiesSolution(resultList);

            var randomLength = random.Next(1, solution1.Length - 1);
            var randomPosition = random.Next(0, solution1.Length - randomLength);

            for (int i = 0; i < randomLength; i++)
            {
                resultList[randomPosition + i] = solution1.GetVertexAt(randomPosition + i);
            }

            return resultCycle;
        }

        public static CitiesSolution PMXSecondStep(CitiesSolution solution1, CitiesSolution solution2, CitiesSolution midSolution)
        {
            for (int i = 0; i < solution2.Length; i++)
            {
                var vertex = solution2.GetVertexAt(i);

                if (midSolution.ContainsVertex(vertex))
                {
                    continue;
                }
                midSolution.SetNextEmptyVertex(vertex);
            }

            return midSolution;
        }

        public static CitiesSolution PMXMutate(CitiesSolution solution1, CitiesSolution solution2)
        {
            var step1 = PMXFirstStep(solution1, solution2);
            var step2 = PMXSecondStep(solution1, solution2, step1);

            return step2;
        }
    }
}
