using System.Collections.Generic;
using System.Linq;
using Kk.CsxCore;

namespace Code.Oop
{
    public static class LangUtils
    {
        public static string JoinToString<T>(this IEnumerable<T> items, string separator = "")
        {
            return string.Join(separator, items.OrEmpty().Select(it => it?.ToString() ?? "null"));
        }
    }
}