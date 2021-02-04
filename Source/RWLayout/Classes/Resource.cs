using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RWLayout.alpha2
{
    /// <summary>
    /// Simple resource accessor wrapper. Reinitializes resource if resource was destroyed (by language switch routine, for example)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>Recmomended to use instead as type if static field.</remarks>
    public class Resource<T> where T : class
    {
        string itemPath;
        T value = null;

        /// <summary>
        /// The resource value
        /// </summary>
        public T Value
        {
            get
            {
                if (value == null)
                {
                    value = ContentFinder<T>.Get(itemPath, true);
                }
                return value;
            }
        }

        /// <summary>
        /// Create resource wrapper with access path
        /// </summary>
        /// <param name="itemPath">resource path</param>
        public Resource(string itemPath)
        {
            this.itemPath = itemPath;
        }

        /// <summary>
        /// Create resource wrapper with resource
        /// </summary>
        /// <param name="value">the resource</param>
        public Resource(T value)
        {
            this.itemPath = value.ToString();
            this.value = value;
        }

        public static implicit operator T(Resource<T> resource) => resource.Value;
        public static implicit operator Resource<T>(T value) => new Resource<T>(value);

    }
}
