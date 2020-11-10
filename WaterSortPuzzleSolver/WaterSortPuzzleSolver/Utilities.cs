using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterSortPuzzleSolver
{
    public static class Utilities
    {
        /// <summary>
        /// Returns the best solution from all solutions.
        /// </summary>
        /// <param name="solution">Best Solution</param>
        /// <param name="allSolutions">Solutions to check</param>
        /// <returns>Number of steps in the solution.</returns>
        public static int GetBestSolution(out List<SolveMove> solution, List<List<SolveMove>> allSolutions)
        {
            if (allSolutions.Count > 0)
            {
                int Best = 0;

                for (int i = 1; i < allSolutions.Count; ++i)
                {
                    if (allSolutions[i].Count < allSolutions[Best].Count)
                    {
                        Best = i;
                    }
                }

                solution = allSolutions[Best];
                return solution.Count;
            }

            solution = null;
            return 0;
        }
    }
}
