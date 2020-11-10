using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterSortPuzzleSolver
{
    public class InputHandler
    {
        /// <summary>
        /// Owning Game Object.
        /// </summary>
        private Game OwningGame;

        /// <summary>
        /// List of possible commands.
        /// </summary>
        private List<InputCommand> Commands;

        public InputHandler(Game owner)
        {
            OwningGame = owner;

            Commands = new List<InputCommand>();

            Action<string[]> RenderAction = (string[] args) =>
            {
                Game.Render(OwningGame.GetTubes());
            };

            Commands.Add(new InputCommand("render", RenderAction));

            Action<string[]> SolveAction = (string[] args) =>
            {
                Console.WriteLine(OwningGame.Solve());
            };

            Commands.Add(new InputCommand("solve", SolveAction));

            Action<string[]> InitializeAction = (string[] args) =>
            {
                if (args.Length == 2)
                {
                    try
                    {
                        OwningGame.Initialize(int.Parse(args[0]), args[1]);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("First argument is not an integer.");
                    }
                }
                else
                {
                    OwningGame.Initialize(args[0]);
                }
            };

            Commands.Add(new InputCommand(new[] { "init", "initialize" }, InitializeAction));

            Action<string[]> AddColorAction = (string[] args) =>
            {
                if (OwningGame.AddColor(args[0], args[1]))
                {
                    Console.WriteLine("New color added. (" + args[0] + ',' + args[1] + ')');
                }
                else
                {
                    Console.WriteLine("Could not add color. (" + args[0] + ',' + args[1] + ')');
                    Console.WriteLine("Either this color or Abbreviation already exists.");
                }
            };

            Commands.Add(new InputCommand("addcolor", AddColorAction));

            Action<string[]> GetColorsAction = (string[] args) =>
            {
                List<Color> Colors = OwningGame.GetColors();

                Console.WriteLine("Available Colors:");

                foreach (Color c in Colors)
                {
                    Console.WriteLine(c.ToString());
                }
            };

            Commands.Add(new InputCommand("getcolors", GetColorsAction));

            Commands.Add(new InputCommand("exit", (string[] args) => OwningGame.Exit()));
        }

        /// <summary>
        /// Called each time input is received to handle the command.
        /// </summary>
        /// <param name="input">Command to handle.</param>
        public void HandleInput(string input)
        {
            string[] args = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (InputCommand c in Commands)
            {
                if (c.Match(args[0]))
                {
                    List<string> l = new List<string>(args);
                    l.RemoveAt(0);
                    c.Execute(l.ToArray());
                    return;
                }
            }

            Console.WriteLine("'" + args[0] + "' is not a valid command.");
        }
    }
}
