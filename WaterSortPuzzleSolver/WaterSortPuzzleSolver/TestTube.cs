using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WaterSortPuzzleSolver
{
    public class TestTube
    {
        /// <summary>
        /// List of the ordered colors inside the tube.
        /// </summary>
        private List<Color> CurrentColors;

        /// <summary>
        /// Owning Game object.
        /// </summary>
        private Game OwningGame;

        /// <summary>
        /// Index of this test tube in OwningGame.Tubes
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="owner"></param>
        public TestTube(Game owner, int index)
        {
            OwningGame = owner;
            Index = index;
            CurrentColors = new List<Color>(Game.GetTubeHeight());
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="initString">Formatted string {color1,color2,color3,color4}</param>
        public TestTube(Game owner, int index, string initString)
        {
            OwningGame = owner;
            Index = index;

            if (initString == "")
            {
                CurrentColors = new List<Color>(Game.GetTubeHeight());

                for (int i = 0; i < Game.GetTubeHeight(); ++i)
                {
                    CurrentColors.Add(new Color());
                }
            }
            else
            {
                string[] colors = initString.Split(',');

                if (colors.Length != Game.GetTubeHeight())
                {
                    Console.WriteLine("TestTube init string is malformed '" + initString + "'");
                    return;
                }

                CurrentColors = new List<Color>(colors.Length);

                foreach (string color in colors)
                {
                    Color c = OwningGame.GetColor(color);

                    if (c == Color.Empty())
                    {
                        Console.WriteLine("TestTube init string '" + initString + "' has unknown color '" + color + "'");
                        return;
                    }
                    else
                    {
                        CurrentColors.Add(c);
                    }
                }
            }
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="other"></param>
        public TestTube(TestTube other)
        {
            OwningGame = other.OwningGame;
            Index = other.Index;
            CurrentColors = new List<Color>(other.CurrentColors.Count);
            
            foreach (Color c in other.CurrentColors)
            {
                CurrentColors.Add(new Color(c));
            }
        }

        public static bool operator ==(TestTube lhs, TestTube rhs)
        {
            if (object.ReferenceEquals(lhs, null))
            {
                return object.ReferenceEquals(rhs, null);
            }
            else if (object.ReferenceEquals(rhs, null))
            {
                return false;
            }

            return lhs.Index == rhs.Index && lhs.ToString() == rhs.ToString();
        }

        public static bool operator !=(TestTube lhs, TestTube rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// Returns a string representation of the test tube.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string RetVal = "";

            foreach (Color c in CurrentColors)
            {
                RetVal += "|  " + c.Abbreviation;

                if (c.Abbreviation.Length == 1)
                {
                    RetVal += "  |\n";
                }
                else
                {
                    RetVal += " |\n";
                }
            }

            return RetVal;
        }

        public List<Color> GetColors()
        {
            return CurrentColors;
        }

        /// <summary>
        /// Returns true if the test tube is empty.
        /// </summary>
        /// <returns>true if the test tube is empty.</returns>
        public bool IsEmpty()
        {
            return GetSpace() == CurrentColors.Count;
        }

        /// <summary>
        /// Returns true if the test tube has no empty space.
        /// </summary>
        /// <returns>true if the test tube is full.</returns>
        public bool IsFull()
        {
            return CurrentColors[0] != Color.Empty();
        }

        /// <summary>
        /// Returns true if the test tube has empty space.
        /// </summary>
        /// <returns>true if the test tube is not full.</returns>
        public bool HasSpace()
        {
            return GetSpace() > 0;
        }

        /// <summary>
        /// Returns true if the test tube is full of a single color.
        /// </summary>
        /// <returns>true if the test tube is solved.</returns>
        public bool IsComplete()
        {
            if (IsFull())
            {
                foreach (Color c in CurrentColors)
                {
                    if (c != CurrentColors[0])
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Does this test tube only have 1 color.
        /// ie: 3 blue, 2 red, etc
        /// </summary>
        /// <returns>true if the tube only has one color</returns>
        public bool HasSingleColor()
        {
            if (!IsEmpty())
            {
                Color c = CurrentColors[CurrentColors.Count - 1];

                for (int i = CurrentColors.Count - 2; i >= 0; --i)
                {
                    if (CurrentColors[i] == Color.Empty())
                    {
                        break;
                    }

                    if (CurrentColors[i] != c)
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the top most color of the tube.
        /// </summary>
        /// <param name="numTogether">How many of the same color are at the top</param>
        /// <returns></returns>
        public Color GetTopColor(out int numTogether)
        {
            numTogether = 0;
            Color top = Color.Empty();

            foreach (Color c in CurrentColors)
            {
                if (c != Color.Empty())
                {
                    if (top == Color.Empty())
                    {
                        top = c;
                    }

                    if (c == top)
                    {
                        ++numTogether;
                    }
                    else 
                    {
                        break;
                    }
                }
            }

            return top;
        }

        /// <summary>
        /// Gets the top most color of the tube.
        /// </summary>
        /// <returns>Top most color, otherwise Color.Empty()</returns>
        public Color GetTopColor()
        {
            foreach (Color c in CurrentColors)
            {
                if (c != Color.Empty())
                {
                    return c;
                }
            }

            return Color.Empty();
        }

        /// <summary>
        /// Returns the number of empty spaces in the test tube.
        /// </summary>
        /// <returns>How many empty spaces the test tube has.</returns>
        public int GetSpace()
        {
            int count = 0;

            foreach (Color c in CurrentColors)
            {
                if (c == Color.Empty())
                {
                    ++count;
                }
            }

            return count;
        }

        /// <summary>
        /// Add a color to the tube.
        /// </summary>
        /// <param name="numAdded">Number of colors added.</param>
        /// <param name="color">Color to add.</param>
        /// <param name="numToAdd">Number of colors to attempt to add.</param>
        /// <returns></returns>
        public bool AddColor(out int numAdded, Color color, int numToAdd)
        {
            numAdded = 0;

            if (!IsFull() && (GetTopColor(out int numTogether) == Color.Empty() || GetTopColor(out numTogether) == color))
            {
                for (int i = CurrentColors.Count - 1; i >= 0; --i)
                {
                    if (CurrentColors[i] == Color.Empty())
                    {
                        for (int j = 0; j < numToAdd && i - j >= 0; ++j)
                        {
                            if (CurrentColors[i - j] == Color.Empty())
                            {
                                CurrentColors[i - j] = color;
                                ++numAdded;
                            }
                        }

                        return true;
                    }                    
                }
            }

            return false;
        }

        /// <summary>
        /// Attempts to remove 'colorToRemove' from the tube.
        /// </summary>
        /// <param name="colorToRemove"></param>
        /// <param name="numToRemove"></param>
        /// <returns>true if successful</returns>
        public bool RemoveColor(Color colorToRemove, int numToRemove)
        {
            for (int i = 0; i < CurrentColors.Count; ++i)
            {
                Color c = CurrentColors[i];

                if (c == Color.Empty())
                {
                    continue;
                }

                if (c == colorToRemove)
                {
                    for (int j = 0; j < numToRemove && i + j < CurrentColors.Count; ++j)
                    {
                        if (CurrentColors[i + j] == colorToRemove)
                        {
                            CurrentColors[i + j] = Color.Empty();
                        }
                        else
                        {
                            return false;
                        }
                    }

                    return true;
                }
                else
                {
                    break;
                }
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            return obj is TestTube tube &&
                   Index == tube.Index;
        }

        public override int GetHashCode()
        {
            int hashCode = 1002823632;
            hashCode = hashCode * -1521134295 + EqualityComparer<List<Color>>.Default.GetHashCode(CurrentColors);
            hashCode = hashCode * -1521134295 + Index.GetHashCode();
            return hashCode;
        }
    }
}
