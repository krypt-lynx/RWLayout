using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Verse;

namespace RWLayout.alpha2
{
    class PropertyWriter
    {
        static public void ApplyProperty(CElement view, XmlElement node, Dictionary<string, object> objectsMap)
        {
            var propNodes = node.SelectSingleNode("properties")?.ChildNodes.AsEnumerable<XmlNode>().Where(x => x.NodeType == XmlNodeType.Element).Cast<XmlElement>() ?? Enumerable.Empty<XmlElement>();
            foreach (var subnode in propNodes)
            {
                var propName = subnode.Name;
                try
                {
                    var propInfo = view.GetType().GetMemberHandler(propName, BindingFlags.Public | BindingFlags.Instance);

                    if (propInfo == null)
                    {
                        Log.Error($"Unable to resolve node {propName} for object {view}");
                        continue;
                    }

                    if (!propInfo.CanWrite)
                    {
                        Log.Error($"Unable to assign property {propName} of object {view}");
                    }

                    var valueType = propInfo.MemberType;
                    var value = TryResolve(subnode, valueType, objectsMap);

                    propInfo.SetValue(view, value);
                }
                catch (Exception e)
                {
                    LogHelper.LogException("Exception thrown during field assignment", e);
                }
            }
        }

        static object TryResolve(XmlElement node, Type hintType, Dictionary<string, object> objectsMap)
        {
            string typeName = node.GetAttribute("Class");
            Type valueType = null;

            if (valueType != null)
            {
                const string ns = "RWLayout.alpha2";
                valueType = GenTypes.GetTypeInAnyAssembly(typeName, ns);
                valueType ??= GenTypes.GetTypeInAnyAssembly($"{ns}.{typeName}", ns);
            }

            valueType ??= hintType;
            valueType = Nullable.GetUnderlyingType(valueType) ?? valueType;

            if (node.HasAttribute("object"))
            {
                string binding = node.GetAttribute("object");
                return Binder.ReadObject(new BindingPath(binding), objectsMap);                
            }
            else
            {
                return ParseFromText(node, valueType);
            }

            return null;
        }

        static object ParseFromText(XmlElement node, Type valueType)
        {
            bool translated = node.GetAttribute("translated")?.ToLowerInvariant() == "true";

            string value = node.InnerText;
            if (translated)
            {
                value = value.Translate();
            }

            if (ParseHelper.HandlesType(valueType))
            {
                return ParseHelper.FromString(value, valueType);
            }
            else
            {
                // Implicit/Explicit cast implementations
                var converter =
                    valueType.GetMethod("op_Implicit", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null) ??
                    valueType.GetMethod("op_Explicit", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null);

                if (converter != null)
                {
                    var result = converter.Invoke(null, new object[] { value });
                    return result;
                }
                else
                {
                    return null;
                }
            }
        }
    }

    static class ParseHelperExtention
    {

    }
}
