using System;

namespace Haden.Library.Attributes
{
    public class AliasAttribute : Attribute
    {
        /// <summary>  
        /// These parameters will become mandatory once have you decided to use this attribute.  
        /// </summary>  
        /// <param name="alias"></param>  
        /// <param name="color"></param>  
        public AliasAttribute(string alias, ConsoleColor color)
        {
            Alias = alias;
            Color = color;
        }

        #region Positional Parameters  
        public string Alias { get; private set; }
        public ConsoleColor Color { get; private set; }
        #endregion

        // Added an optional parameter.
        public string AlternativeName { get; set; }
    }
}
