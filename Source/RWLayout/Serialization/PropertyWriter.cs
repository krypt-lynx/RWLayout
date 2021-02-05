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
            var propNodes = node.SelectSingleNode("properties")?.ChildNodes.WhereTypeIs<XmlElement>() ?? Enumerable.Empty<XmlElement>();
            foreach (var subnode in propNodes)
            {
                try
                {
                    if (subnode.HasAttribute("bind"))
                    {
                        BindProperty(view, objectsMap, subnode);
                    }
                    else
                    {
                        AssignProperty(view, objectsMap, subnode);
                    }
                }
                catch (Exception e)
                {
                    LogHelper.LogException($"Exception thrown during field assignment; target object: \"{view}\"; node: \"{subnode.OuterXml}\"; exception", e);
                }
            }
        }

        private static bool IsCompatibleBindable(Type bindableType, Type propType)
        {
            if (!bindableType.IsGenericType ||
                !bindableType.GetGenericTypeDefinition().IsAssignableFrom(typeof(Bindable<>)))
                return false;

            var args = bindableType.GetGenericArguments();

            return args.Length == 1 && args[0] == propType;
        }

        private static void BindProperty(CElement view, Dictionary<string, object> objectsMap, XmlElement node)
        {
            // todo exceptions
            var propName = node.Name;
            var srcPath = new BindingPath(node.GetAttribute("bind"));

            object srcValue = objectsMap[srcPath.Object];
            object srcObject = null;
            Type srcType = null;
            if (srcValue is Type)
            {
                srcType = (Type)srcValue;
                srcObject = null;
            } else
            {
                srcType = srcValue.GetType();
                srcObject = srcValue;
            }

            var srcProp = srcType.GetMemberHandler(srcPath.Member, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

            if (srcProp == null)
            {
                Log.Error($"Unable to assign property \"{propName}\" of object \"{view}\": source object \"{srcPath}\" does not exists");
                return;
            }

            var dstProp = view.GetType().GetMemberHandler(propName, BindingFlags.Public | BindingFlags.Instance);

            if (dstProp != null && !IsCompatibleBindable(dstProp.TargetType, srcProp.TargetType))
            {
                dstProp = view.GetType().GetMemberHandler(propName + "Prop", BindingFlags.Public | BindingFlags.Instance);
            }

            if (dstProp != null && IsCompatibleBindable(dstProp.TargetType, srcProp.TargetType))
            {
                var dstObj = dstProp.GetValue(view);
                dstProp.TargetType.GetMethod(nameof(Bindable<object>.Bind), BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(dstObj, new object[] { srcObject, srcProp });
                $"Binded {srcProp} of {srcValue} to {dstProp}".Log();
            }
            else
            {
                if (dstProp == null)
                {
                    Log.Error($"Unable to assign property \"{propName}\" of object \"{view}\": property \"{propName}\" does not exists");
                }
                else
                {
                    Log.Error($"Unable to assign property \"{propName}\" of object \"{view}\": \"{srcValue}\" is not of compatible type");
                }
            }
        }

        private static void AssignProperty(CElement view, Dictionary<string, object> objectsMap, XmlElement node)
        {
            var propName = node.Name;

            var propInfo = view.GetType().GetMemberHandler(propName, BindingFlags.Public | BindingFlags.Instance);

            if (propInfo == null)
            {
                Log.Error($"Unable to resolve node {propName} for object {view}");
                return;
            }

            if (!propInfo.CanWrite)
            {
                Log.Error($"Unable to assign property {propName} of object {view}");
                return;
            }

            var valueType = propInfo.TargetType;
            var value = TryResolve(node, valueType, objectsMap);
            $"Assigned {value} to {propName}".Log();
            propInfo.SetValue(view, value);
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
