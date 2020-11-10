using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterSortPuzzleSolver
{
    public class Color
    {
        public string Name { get; }

        public string Abbreviation { get; }

        /// <summary>
        /// Constructor.
        /// Creates an empty color.
        /// </summary>
        public Color()
        {
            Name = "EMPTY";
            Abbreviation = "E";
        }

        /// <summary>
        /// Constructor.
        /// Creates a named color.
        /// </summary>
        /// <param name="name">Color Name</param>
        /// <param name="Abbreviation">Color Abbreviation</param>
        public Color(string name, string abbreviation)
        {
            Name = name.ToUpper();
            Abbreviation = abbreviation.ToUpper();
        }

        /// <summary>
        /// Copy Constructor.
        /// </summary>
        /// <param name="other"></param>
        public Color(Color other)
        {
            Name = other.Name;
            Abbreviation = other.Abbreviation;
        }

        public override string ToString()
        {
            return Name + ' ' + Abbreviation;
        }

        public static bool operator ==(Color lhs, Color rhs)
        {
            return lhs.Abbreviation == rhs.Abbreviation;
        }

        public static bool operator !=(Color lhs, Color rhs)
        {
            return lhs.Abbreviation != rhs.Abbreviation;
        }

        /// <summary>
        /// Check if this color matches the given name or Abbreviation.
        /// </summary>
        /// <param name="nameOrAbbreviation">Color Name or Color Abbreviation</param>
        /// <returns>true if it is a match.</returns>
        public bool Match(string nameOrAbbreviation)
        {
            return Name == nameOrAbbreviation.ToUpper() || Abbreviation == nameOrAbbreviation.ToUpper();
        }

        public static Color Empty()
        {
            return new Color();
        }

        public override bool Equals(object obj)
        {
            return obj is Color color &&
                   Name == color.Name &&
                   Abbreviation == color.Abbreviation;
        }

        public override int GetHashCode()
        {
            int hashCode = 1187235447;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Abbreviation);
            return hashCode;
        }
    }
}
