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
    class Binder
    {
        static public void ApplyProperties(CElement view, PropertyPrototype[] properties, Dictionary<string, object> objectsMap)
        {
            foreach (var property in properties)
            {
                try
                {
                    switch (property.Kind)
                    {
                        case PropertyPrototype.PropertyPrototypeKind.AssignFrom:
                            AssignPropertyFrom(view, objectsMap, property);
                            break;
                        case PropertyPrototype.PropertyPrototypeKind.BindValue:
                            BindProperty(view, objectsMap, property);
                            break;
                        case PropertyPrototype.PropertyPrototypeKind.TextRepresentation:
                            AssignPropertyText(view, objectsMap, property);
                            break;
                    }
                }
                catch (Exception e)
                {
                    LogHelper.LogException($"Exception thrown during field assignment; target object: \"{view}\"; property: \"{property}\"; exception", e);
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

        private static void BindProperty(CElement view, Dictionary<string, object> objectsMap, PropertyPrototype property)
        {
            // todo exceptions
            var propName = property.Name;
            var srcPath = property.Path;

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

            var srcProp = srcType.GetMember(srcPath.Member, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).FirstOrDefault(); // todo: propper overload resolving

            if (srcProp == null)
            {
                Log.Error($"Unable to assign property \"{propName}\" of object \"{view}\": source object \"{srcPath}\" does not exists");
                return;
            }

            var dstProp = view.GetType().GetMember(propName, BindingFlags.Public | BindingFlags.Instance).FirstOrDefault(); // todo: propper overload resolving

            if (dstProp != null && !IsCompatibleBindable(dstProp.MemberType(), srcProp.MemberType()))
            {
                dstProp = view.GetType().GetMember(propName + "Prop", BindingFlags.Public | BindingFlags.Instance).FirstOrDefault(); // todo: propper overload resolving
            }

            if (dstProp != null && IsCompatibleBindable(dstProp.MemberType(), srcProp.MemberType()))
            {
                var dstObj = dstProp.GetValue(view);
                dstProp.MemberType().GetMethod(nameof(Bindable<object>.Bind), BindingFlags.Instance | BindingFlags.NonPublic)
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

        private static void AssignPropertyFrom(CElement view, Dictionary<string, object> objectsMap, PropertyPrototype property)
        {
            var propName = property.Name;
            var propInfo = view.GetType().GetMember(propName, BindingFlags.Public | BindingFlags.Instance).FirstOrDefault();

            if (propInfo == null)
            {
                Log.Error($"Unable to resolve node {propName} for object {view}");
                return;
            }

            if (!propInfo.CanWrite())
            {
                Log.Error($"Unable to assign property {propName} of object {view}");
                return;
            }

            var valueType = property.TypeHint ?? propInfo.MemberType();
            valueType = Nullable.GetUnderlyingType(valueType) ?? valueType;                       

            var value = PropertyWriter.ReadObject(property.Path, objectsMap);
            propInfo.SetValue(view, value);

            $"Assigned {value} to {propName}".Log();
        }


        private static void AssignPropertyText(CElement view, Dictionary<string, object> objectsMap, PropertyPrototype property)
        {
            var propName = property.Name;
            var propInfo = view.GetType().GetMember(propName, BindingFlags.Public | BindingFlags.Instance).FirstOrDefault(); // todo:

            if (propInfo == null)
            {
                Log.Error($"Unable to resolve node {propName} for object {view}");
                return;
            }

            if (!propInfo.CanWrite())
            {
                Log.Error($"Unable to assign property {propName} of object {view}");
                return;
            }

            var valueType = property.TypeHint ?? propInfo.MemberType();
            valueType = Nullable.GetUnderlyingType(valueType) ?? valueType;

            var value = ParseFromText(property.Value, property.Translate, valueType);
            propInfo.SetValue(view, value);

            $"Assigned {value} to {propName}".Log();
        }

        static object ParseFromText(string value, bool translate, Type valueType)
        {
            if (translate)
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
