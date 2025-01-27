using System;
using System.Collections.Generic;
using System.Linq;
using NLua;

namespace Haden.Library
{
    public class LuaUnderworld : Lua
    {
        public string Objects { get; set; }
        public string Print { get; set; }
        public string Result { get; set; }
        public Lua LuaObject { get; set; }
        public LuaUnderworld(string path) 
        {
            LuaObject = new Lua();
            RunLua(path);
        }

        private void RunLua(string path)
        {
            try
            {
                LuaObject.DoFile(path);

                // SumUp(a, b)
                var result = LuaObject.DoString("return SumUp(56, 112)");
                Result = "56 + 112 = " + result.First().ToString();

                // Print string
                var print = LuaObject.DoString("return TestPrint()");
                Print = "Printing - " + print.First().ToString();

                // GetTable()           
                var objects = LuaObject.DoString("return GetTable()"); // Array of objects that are printed randomly.
                foreach (LuaTable table in objects)
                    foreach (KeyValuePair<object, object> i in table)
                        Objects = $"{i.Key.ToString()}: {i.Value.ToString()}";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
            }
        }
    }
}
