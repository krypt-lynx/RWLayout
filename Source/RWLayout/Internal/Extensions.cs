using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RWLayout.alpha2
{
    public static class XmlNodeListExtentions
    {
        /// <summary>
        /// Converts IEnumerable to IEnumerable&lt;T&gt;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<T> AsEnumerable<T>(this IEnumerable list)
        {
            foreach (var node in list)
            {
                yield return (T)node;
            }
        }

    }
}
