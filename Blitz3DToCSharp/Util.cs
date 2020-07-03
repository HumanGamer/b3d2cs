using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blitz3DToCSharp
{
    public static class Util
    {
        public static string GetBaseName(string name, bool trimType = true)
        {
            int index = name.IndexOf('(');
            if (index != -1)
                name = name.Substring(0, index).Trim();

            if (trimType)
            {
                char last = name.Last();
                if (last == '#' || last == '$' || last == '%')
                    name = name.Substring(0, name.Length - 1);
            }

            return name;
        }

        public static BlitzType GetType(string name)
        {
            switch(GetBaseName(name, false).Last())
            {
                case '#':
                    return BlitzType.Float;
                case '$':
                    return BlitzType.String;
                case '%':
                default:
                    return BlitzType.Integer;
            }
        }
    }
}
