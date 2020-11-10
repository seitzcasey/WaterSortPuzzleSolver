using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterSortPuzzleSolver
{
    class InputCommand
    {
        /// <summary>
        /// List of possible command names.
        /// </summary>
        public string[] Name { get; }
        
        /// <summary>
        /// Action for this command to execute.
        /// </summary>
        public Action<string[]> Action { get; }


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Single name to call this command.</param>
        /// <param name="action">Action for this command to execute.</param>
        public InputCommand(string name, Action<string[]> action)
            : this(new[] { name }, action)
        { 
        
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="names">Multiple names to call this command.</param>
        /// <param name="action">Action for this command to execute.</param>
        public InputCommand(string[] names, Action<string[]> action)
        {
            Name = names;
            Action = action;
        }

        /// <summary>
        /// Check to see if the input name matches this command.
        /// </summary>
        /// <param name="name">Name to match.</param>
        /// <returns>true if they match.</returns>
        public bool Match(string name)
        {
            string lName = name.ToLower();

            foreach (string s in Name)
            {
                if (s == lName)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Execute this command.
        /// </summary>
        /// <param name="args">Additional arguments.</param>
        public void Execute(string[] args)
        {
            Action.Invoke(args);
        }
    }
}
