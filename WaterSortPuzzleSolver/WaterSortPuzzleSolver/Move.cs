using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterSortPuzzleSolver
{
    public class SolveMove
    {
        /// <summary>
        /// Index of the starting TestTube.
        /// </summary>
        public int StartIndex { get; }

        /// <summary>
        /// Index of the ending TestTube.
        /// </summary>
        public int EndIndex { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="startIndex">Index of starting TestTube.</param>
        /// <param name="endIndex">Index of ending TestTube.</param>
        public SolveMove(int startIndex, int endIndex)
        {
            StartIndex = startIndex;
            EndIndex = endIndex;
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="other">Object to copy</param>
        public SolveMove(SolveMove other)
        {
            StartIndex = other.StartIndex;
            EndIndex = other.EndIndex;
        }

        /// <summary>
        /// Return string representation of this move.
        /// </summary>
        /// <returns>startIndex->endIndex</returns>
        public override string ToString()
        {
            return StartIndex.ToString() + "->" + EndIndex.ToString();
        }

        public static bool operator ==(SolveMove lhs, SolveMove rhs)
        {
            return lhs.StartIndex == rhs.StartIndex &&
                lhs.EndIndex == rhs.EndIndex;
        }

        public static bool operator !=(SolveMove lhs, SolveMove rhs)
        {
            return lhs.StartIndex != rhs.StartIndex ||
                lhs.EndIndex != rhs.EndIndex;
        }

        public override bool Equals(object obj)
        {
            return (SolveMove)obj == this;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
