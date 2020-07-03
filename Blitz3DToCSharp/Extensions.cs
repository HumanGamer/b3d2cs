using System;
using System.Collections.Generic;
using System.Text;

namespace Blitz3DToCSharp
{
    public static class Extensions
    {
        public static bool ContainsIgnoreCase(this string[] self, string compare)
        {
            foreach (var f in self)
            {
                if (string.Equals(f, compare, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        public static bool ContainsIgnoreCase(this List<string> self, string compare)
        {
            foreach (var f in self)
            {
                if (string.Equals(f, compare, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }
    }
}
