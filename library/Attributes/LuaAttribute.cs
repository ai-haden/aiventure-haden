using System;

namespace Haden.Library.Attributes
{
    //[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class LuaAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LuaAttribute"/> class.
        /// </summary>
        /// <param name="module">The lua module to call. Typically the filename.</param>
        public LuaAttribute(string module)
        {
            Module = module;
            Version = 1.0;
        }

        #region Positional Parameters  
        public string Module { get; private set; }
        public double Version { get; private set; }
        #endregion
    }
}
