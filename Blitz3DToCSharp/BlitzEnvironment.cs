using System;
using System.Collections.Generic;
using System.Text;

namespace Blitz3DToCSharp
{
    public class BlitzEnvironment
    {
        public string[] Constants
        {
            get;
            private set;
        }

        public string[] Globals
        {
            get;
            private set;
        }

        public string[] Arrays
        {
            get;
            private set;
        }

        public string[] Functions
        {
            get;
            private set;
        }

        public BlitzEnvironment(string[] constants, string[] globals, string[] arrays, string[] functions)
        {
            Constants = constants;
            Globals = globals;
            Arrays = arrays;
            Functions = functions;
        }

        public BlitzEnvironment(params BlitzBasicFile[] files)
        {
            List<string> constants = new List<string>();
            List<string> globals = new List<string>();
            List<string> arrays = new List<string>();
            List<string> functions = new List<string>();

            foreach (var file in files)
            {
                foreach (var constant in constants)
                {
                    if (!constants.Contains(constant))
                        constants.Add(constant);
                }

                foreach (var global in globals)
                {
                    if (!globals.Contains(global))
                        globals.Add(global);
                }

                foreach (var array in arrays)
                {
                    if (!arrays.Contains(array))
                        arrays.Add(array);
                }

                foreach (var function in functions)
                {
                    if (!functions.Contains(function))
                        functions.Add(function);
                }
            }

            Constants = constants.ToArray();
            Globals = globals.ToArray();
            Arrays = arrays.ToArray();
            Functions = functions.ToArray();
        }
    }
}
