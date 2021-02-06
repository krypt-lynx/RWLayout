using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RWLayout.alpha2
{
    static class PropertyWriter
    {
        /// <summary>
        /// Assigns valus from source to destination
        /// </summary>
        /// <param name="binding">binding information</param>
        /// <param name="objects">id to object map</param>
        internal static void Assign(BindingPrototype binding, Dictionary<string, object> objects)
        {
            object srcObj = ReadObject(binding.Source, objects);

            object dstObj = objects[binding.Target.Object];
            var dstProp = dstObj.GetType().GetMemberHandler(binding.Target.Member, BindingFlags.Public | BindingFlags.Instance);
            if (dstProp != null)
            {
                $"Assigning {dstObj} to {srcObj}".Log();
                // todo: T? to T assignment
                dstProp.SetValue(dstObj, srcObj);
            }
            else
            {
                throw new InvalidOperationException($"object {dstObj} with id \"{binding.Target.Object}\" have no property \"{binding.Target.Member}\"");
            }
        }

        /// <summary>
        /// Reads the value at provided path
        /// </summary>
        /// <param name="path">path</param>
        /// <param name="objects">id to object map</param>
        /// <returns>value read</returns>
        internal static object ReadObject(BindingPath path, Dictionary<string, object> objects)
        {
            object obj = objects[path.Object];
            if (path.Member != null)
            {
                var srcProp = obj.GetType().GetMemberHandler(path.Member, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                if (srcProp != null)
                {
                    obj = srcProp.GetValue(obj);
                }
                else
                {
                    throw new InvalidOperationException($"object {obj} with id \"{path.Object}\" have no property \"{path.Member}\"");
                }
            }
            return obj;
        }
    }
}
