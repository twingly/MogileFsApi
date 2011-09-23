using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Primelabs.Twingly.MogileFsApi.Utils
{
    public static class StringHelpers
    {
        public static string Join(this string separator, IEnumerable<string> vals)
        {
            var retval = new StringBuilder();
            foreach (var val in vals) {
                if (retval.Length != 0)
                    retval.Append(separator);
                retval.Append(val.ToString());
            }
            return retval.ToString();
        }
    }
}
