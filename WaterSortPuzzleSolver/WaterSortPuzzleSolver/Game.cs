using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WaterSortPuzzleSolver
{
    public class Game
    {
        /// <summary>
        /// Filepath for the color file.
        /// </summary>
        public static string ColorPath = "Colors.txt";

        /// <summary>
        /// Available Colors
        /// </summary>
        private List<Color> Colors;

        /// <summary>
        /// List of tubes in the current game.
        /// </summary>
        private List<TestTube> Tubes;

        /// <summary>
        /// Handler for parsing and executing CLI.
        /// </summary>
        private InputHandler IOHandler;

        public bool Running { get; private set; }

        /// <summary>
        /// Constructor.
        /// Initializes Colors from disk.
        /// </summary>
        public Game()
        {
            Colors = new List<Color>();

            Tubes = new List<TestTube>();

            IOHandler = new InputHandler(this);

            if (File.Exists(ColorPath))
            {

                using (StreamReader sr = File.OpenText(ColorPath))
                {
                    string line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] l = line.Split(' ');
                        Colors.Add(new Color(l[0], l[1]));
                    }
                }
            }
        }

        /// <summary>
        /// Destructor.
        /// Writes out colors to disk.
        /// </summary>
        ~Game()
        {
            using (StreamWriter sw = new StreamWriter(ColorPath))
            {
                foreach (Color c in Colors)
                {
                    sw.WriteLine(c.ToString());
                }
            }
        }

        /// <summary>
        /// Starts the game and begins handling input.
        /// </summary>
        public void Begin()
        {
            Running = true;

            while (Running)
            {
                Console.WriteLine("Enter a command.");
                string Input = Console.ReadLine();
                IOHandler.HandleInput(Input);
            }
        }

        /// <summary>
        /// Exit the game.
        /// </summary>
        public void Exit()
        {
            Running = false;
        }

        /// <summary>
        /// Initializes a setup.
        /// </summary>
        /// <param name="initString">Formatted string [{tube1}|{tube2}|{...}]</param>
        public void Initialize(string initString)
        {
            if (initString[0] != '[' || initString[initString.Length - 1] != ']')
            {
                Console.WriteLine("Invalid initialize string passed. Missing square bracket.\n'" + initString + "'");
                return;
            }

            string[] TubeStrings = initString.Substring(1, initString.Length - 2).Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            Tubes = new List<TestTube>(TubeStrings.Length);

            for (int i = 0; i < TubeStrings.Length; ++i) 
            {
                Initialize(i + 1, TubeStrings[i]);
            }
        }

        /// <summary>
        /// Initializes a certain tube.
        /// </summary>
        /// <param name="tubeIndex">Which tube to initialize. [1, ...)</param>
        /// <param name="tubeInitString">Formatted string {color1,color2,color 3,color4} Color 1 is the top.</param>
        public void Initialize(int tubeIndex, string tubeInitString)
        {
            if (tubeInitString[0] != '{' || tubeInitString[tubeInitString.Length - 1] != '}')
            {
                Console.WriteLine("Invalid initialize string passed. Missing curly brace.\n'" + tubeInitString + "'");
                return;
            }

            if (Tubes.Count >= tubeIndex)
            {
                Tubes[tubeIndex - 1] = new TestTube(this, tubeIndex, tubeInitString.Substring(1, tubeInitString.Length - 2));;
            }
            else
            {
                int NumToCreate = tubeIndex - Tubes.Count;
                
                for (int i = 0; i < NumToCreate; ++i)
                {
                    Tubes.Add(new TestTube(this, Tubes.Count));
                }

                Initialize(tubeIndex, tubeInitString);
            }
        }

        /// <summary>
        /// Generates ascii art to "render" the game
        /// and writes it to the console.
        /// </summary>
        public static void Render(List<TestTube> Tubes)
        {
            if (Tubes.Count == 0) return;

            List<string> StringifiedTubes = new List<string>(Tubes.Count);

            foreach (TestTube t in Tubes)
            {
                StringifiedTubes.Add(t.ToString());
            }

            int NumLines = StringifiedTubes[0].Split('\n').Length;
            List<string> FinalLines = new List<string>(NumLines);

            for (int i = 0; i < NumLines; ++i)
            {
                FinalLines.Add("");
            }

            foreach (string tube in StringifiedTubes)
            {
                string[] TubeLines = tube.Split('\n');

                for (int i = 0; i < TubeLines.Length; ++i)
                {
                    FinalLines[i] += TubeLines[i] + '\t';
                }
            }

            string RetVal = "";

            foreach (string line in FinalLines)
            {
                RetVal += line + '\n';
            }

            Console.WriteLine(RetVal);
        }

        /// <summary>
        /// Validates there are a correct number of colors across the tubes.
        /// </summary>
        /// <returns>true if the tubes are valid.</returns>
        public static Color[] ValidateTubes(List<TestTube> tubes)
        {
            Dictionary<Color, int> count = new Dictionary<Color, int>();

            foreach (TestTube t in tubes)
            {
                foreach (Color c in t.GetColors())
                {
                    if (c == Color.Empty()) continue;

                    if (count.ContainsKey(c))
                    {
                        if (--count[c] == 0)
                        {
                            count.Remove(c);
                        }
                    }
                    else
                    {
                        count[c] = Game.GetTubeHeight() - 1;
                    }
                }
            }

            if (count.Count != 0)
            {
                Color[] RetVal = new Color[count.Count];
                count.Keys.CopyTo(RetVal, 0);
                return RetVal;
            }

            return null;
        }

        /// <summary>
        /// Solve the puzzle.
        /// Generates a string of moves to make.
        /// StartTube->EndTube ie: 1->3, 2->7, ...
        /// </summary>
        /// <returns></returns>
        public string Solve()
        {
            Color[] MissingColors = ValidateTubes(Tubes);
            
            if (MissingColors == null)
            {
                Func<List<SolveMove>, string> BuildMoveString = moves =>
                {
                    string RetVal = "";
                    int Break = 4;
                    int b = Break;

                    foreach (SolveMove m in moves)
                    {
                        RetVal += m.ToString();

                        if (--b == 0)
                        {
                            RetVal += '\n';
                            b = Break;
                        }
                        else
                        {
                            RetVal += '\t';
                        }
                    }

                    return RetVal;
                };

                List<SolveMove> Solution = new List<SolveMove>();
                List<List<TestTube>> StateCache = new List<List<TestTube>>();

                Console.WriteLine("Attempting to solve the puzzle.");

                Solve_R(ref Solution, Tubes, new List<SolveMove>(), ref StateCache);

                if (Solution.Count > 0)
                {
                    return BuildMoveString(Solution);
                }                

                return "No solution found.";
            }

            return "Invalid color count.";
        }

        /// <summary>
        /// Recursive solve function
        /// </summary>
        /// <param name="solution">List of moves to solve the puzzle</param>
        /// <param name="currentState">Setup of the tubes when entering the function</param>
        /// <param name="inMoves">List of moves that have already been completed</param>
        /// <param name="stateCache">List of already evaluated states.</param>
        private void Solve_R(ref List<SolveMove> solution, List<TestTube> currentState, List<SolveMove> inMoves, ref List<List<TestTube>> stateCache)
        {
            // We already have a better solution
            int count = solution.Count;
            if (count > 0 && inMoves.Count > count)
            {
                return;
            }

            Func<List<TestTube>, List<TestTube>, bool> IsEqual = (lhs, rhs) =>
            {
                for (int i = 0; i < lhs.Count; ++i)
                {
                    if (lhs[i] != rhs[i])
                    {
                        return false;
                    }
                }

                return true;
            };

            // Make sure we haven't already evaluated this state.
            foreach (List<TestTube> state in stateCache)
            {
                if (IsEqual(currentState, state))
                {
                    return;
                }
            }

            stateCache.Add(currentState);

            // Returns true if we have solved the puzzle.
            Func<bool> Finished = () =>
            {
                foreach (TestTube t in currentState)
                {
                    if (!t.IsComplete() && !t.IsEmpty())
                    {
                        return false;
                    }
                }

                return true;
            };

            if (Finished())
            {
                if (solution.Count < inMoves.Count)
                {
                    solution = inMoves;
                }

                return;
            }

            // Returns a copy of the given list of tubes.
            Func<List<TestTube>, List<TestTube>> Copy = list =>
            {
                List<TestTube> RetVal = new List<TestTube>(list.Count);

                foreach (TestTube t in list)
                {
                    RetVal.Add(new TestTube(t));
                }

                return RetVal;
            };

            // Moves a color from tube 'start to tube 'end'
            Func<TestTube, TestTube, List<SolveMove>> Move = (start, end) =>
            {
                if (start.IsEmpty() || start.IsComplete()) return inMoves;

                SolveMove NewMove = new SolveMove(start.Index, end.Index);

                if (inMoves.Count > 0)
                {
                    SolveMove LastMove = inMoves[inMoves.Count - 1];
                    SolveMove m = new SolveMove(NewMove.EndIndex, NewMove.StartIndex);

                    if (LastMove == m)
                    {
                        return inMoves;
                    }

                    if (inMoves.Count > 1)
                    {
                        SolveMove MoveBeforeLast = inMoves[inMoves.Count - 2];

                        if (MoveBeforeLast == m)
                        {
                            return inMoves;
                        }
                    }
                }

                Color c = start.GetTopColor(out int NumAvailable);

                if (end.AddColor(out int NumAdded, c, NumAvailable))
                {
                    start.RemoveColor(c, NumAdded);
                    List<SolveMove> moves = new List<SolveMove>(inMoves.Count);

                    foreach (SolveMove m in inMoves)
                    {
                        moves.Add(new SolveMove(m));
                    }

                    moves.Add(NewMove);
                    return moves;
                }

                return inMoves;
            };

            //Game.Render(currentState);

            Color[] MissingColors = ValidateTubes(currentState);
            
            if (MissingColors != null)
            {
                int x = 0;
            }

            List<TestTube> stateCopy = Copy(currentState);

            // Finds a tube that has empty space. null if one does not exist.
            Func<List<TestTube>, List<TestTube>, TestTube> FindTubeWithSpace = (list, ignore) =>
            {
                TestTube BestTube = null;

                foreach (TestTube t in list)
                {
                    int Space = t.GetSpace();
                    if (Space > 0 && !t.IsEmpty() && !ignore.Contains(t))
                    {
                        if (Space == 3)
                        {
                            return t;
                        }

                        if (BestTube == null || t.GetSpace() > BestTube.GetSpace())
                        {
                            BestTube = t;
                        }
                    }
                }

                return BestTube;
            };

            List<TestTube> TubeWithSpaceIgnore = new List<TestTube>();
            TestTube TubeWithSpace = FindTubeWithSpace(stateCopy, TubeWithSpaceIgnore);

            while (TubeWithSpace != null)
            {
                // Finds a tube with a matching top color of tube 'target', otherwise null.
                Func<List<TestTube>, TestTube, List<TestTube>, TestTube> FindTubeWithMatchingTopColor = (list, target, ignore) =>
                {
                    Color targetColor = target.GetTopColor();

                    foreach (TestTube t in list)
                    {
                        if (t.GetTopColor() == targetColor && t != target)
                        {
                            if (t.HasSingleColor())
                            {
                                if (target.HasSingleColor() && (t.GetSpace() > target.GetSpace() || (t.GetSpace() == 1 && target.GetSpace() == 1)))
                                {
                                    return t;
                                }
                            }
                            else if (!ignore.Contains(t))
                            {
                                return t;
                            }
                        }
                    }

                    return null;
                };

                List<TestTube> SourceTubeIgnore = new List<TestTube>();
                TestTube SourceTube = FindTubeWithMatchingTopColor(stateCopy, TubeWithSpace, SourceTubeIgnore);
                // Cache the TubeWithSpace for if the recursive call fails.
                // It will have changed and we need the original.
                TestTube TubeWithSpaceCache = new TestTube(TubeWithSpace);

                while (SourceTube != null)
                {
                    List<SolveMove> moves = Move(SourceTube, TubeWithSpace);

                    if (moves != inMoves)
                    {
                        Solve_R(ref solution, stateCopy, moves, ref stateCache);

                        if (solution.Count > 0)
                        {
                            return;
                        }
                        
                        break;
                    }

                    stateCopy = Copy(currentState);
                    SourceTubeIgnore.Add(SourceTube);
                    SourceTube = FindTubeWithMatchingTopColor(stateCopy, TubeWithSpace, SourceTubeIgnore);
                }

                stateCopy = Copy(currentState);
                TubeWithSpaceIgnore.Add(TubeWithSpaceCache);
                TubeWithSpace = FindTubeWithSpace(stateCopy, TubeWithSpaceIgnore);
            }

            // Returns an empty tube if one exists, otherwise null.
            Func<List<TestTube>, List<TestTube>, TestTube> FindEmptyTube = (list, ignore) =>
            {
                foreach (TestTube t in list)
                {
                    if (t.IsEmpty() && !ignore.Contains(t))
                    {
                        return t;
                    }
                }

                return null;
            };

            List<TestTube> EmptyTubeIgnore = new List<TestTube>();
            TestTube EmptyTube = FindEmptyTube(stateCopy, EmptyTubeIgnore);

            while (EmptyTube != null)
            {
                // Returns a tube that is not a solid color, otherwise null.
                Func<List<TestTube>, List<TestTube>, TestTube> FindTubeWithColorToMove = (list, ignore) =>
                {
                    TestTube BestTube = null;

                    foreach (TestTube t in list)
                    {
                        if (!t.IsEmpty() && !t.HasSingleColor() && !ignore.Contains(t))
                        {
                            if (BestTube == null)
                            {
                                BestTube = t;
                            }
                            else
                            {
                                t.GetTopColor(out int NumTogether);
                                BestTube.GetTopColor(out int BestTogether);

                                if (NumTogether > BestTogether)
                                {
                                    BestTube = t;
                                }
                            }
                        }
                    }

                    return BestTube;
                };

                List<TestTube> TubeToRemoveIgnore = new List<TestTube>();
                TestTube TubeToRemoveFrom = FindTubeWithColorToMove(stateCopy, TubeToRemoveIgnore);
                // Cache the EmptyTube for if the recursive call fails.
                // It will have changed and we need the original.
                TestTube EmptyTubeCache = new TestTube(EmptyTube);

                while (TubeToRemoveFrom != null)
                {
                    List<SolveMove> moves = Move(TubeToRemoveFrom, EmptyTube);

                    if (moves != inMoves)
                    {
                        Solve_R(ref solution, stateCopy, moves, ref stateCache);

                        if (solution.Count > 0)
                        {
                            return;
                        }
                    }

                    stateCopy = Copy(currentState);
                    EmptyTube = stateCopy[EmptyTubeCache.Index - 1];
                    TubeToRemoveIgnore.Add(TubeToRemoveFrom);
                    TubeToRemoveFrom = FindTubeWithColorToMove(stateCopy, TubeToRemoveIgnore);
                }

                stateCopy = Copy(currentState);
                EmptyTubeIgnore.Add(EmptyTubeCache);
                EmptyTube = FindEmptyTube(stateCopy, EmptyTubeIgnore);
            }

            //Console.WriteLine("Path Failed. " + inMoves.Count.ToString());
            return;
        }

        /// <summary>
        /// Returns a list of Tubes for the current setup.
        /// </summary>
        /// <returns>List of tubes.</returns>
        public List<TestTube> GetTubes()
        {
            return Tubes;
        }

        /// <summary>
        /// Retrieve the list of available colors.
        /// </summary>
        /// <returns>List of available colors.</returns>
        public List<Color> GetColors()
        {
            return Colors;
        }

        /// <summary>
        /// Fetches an existing color by name or Abbreviation
        /// </summary>
        /// <param name="nameOrAbbreviation">Color Name or Abbreviation</param>
        /// <returns>Color or Empty Color if it doesn't exist.</returns>
        public Color GetColor(string nameOrAbbreviation)
        {
            foreach (Color c in Colors)
            {
                if (c.Match(nameOrAbbreviation))
                {
                    return c;
                }
            }

            return Color.Empty();
        }

        /// <summary>
        /// Adds a color to the available color list.
        /// </summary>
        /// <param name="colorName">Name of the color</param>
        /// <param name="ColorAbbreviation">Abbreviation for the color</param>
        /// <returns>true if Name or Abbreviation wasn't already taken</returns>
        public bool AddColor(string colorName, string colorAbbreviation)
        {
            foreach (Color c in Colors)
            {
                if (c.Name == colorName ||
                    c.Abbreviation == colorAbbreviation)
                {
                    return false;
                }
            }

            Colors.Add(new Color(colorName, colorAbbreviation));
            return true;
        }

        /// <summary>
        /// How many colors fit in a tube.
        /// </summary>
        /// <returns></returns>
        public static int GetTubeHeight()
        {
            return 4;
        }
    }
}
