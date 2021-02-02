using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RWLayout.alpha2
{
    class MemberHandler
    {
        private PropertyInfo prop = null;
        private FieldInfo field = null;

        public MemberHandler(PropertyInfo prop)
        {
            this.prop = prop;
        }

        public MemberHandler(FieldInfo field)
        {
            this.field = field;
        }

        public bool CanWrite
        {
            get
            {
                return prop?.CanWrite ?? (field != null);
            }
        }

        public Type MemberType
        {
            get
            {
                return prop?.PropertyType ?? field?.FieldType;
            }
        }

        internal object GetValue(object obj)
        {
            if (prop != null)
            {
                return prop.GetValue(obj);
            } 
            else if (field != null)
            {
                return field.GetValue(obj);
            }
            else
            {
                return null;
            }
        }

        internal void SetValue(object obj, object value)
        {
            if (prop != null)
            {
                prop.SetValue(obj, value);
            }
            else if (field != null)
            {
                field.SetValue(obj, value);
            }
        }
    }

    static class GenReflection
    {
        public static MemberHandler GetMemberHandler(this Type type, string name, BindingFlags bindingAttr)
        {
            var prop = type.GetProperty(name, bindingAttr);
            if (prop != null)
            {
                return new MemberHandler(prop);
            } 
            else
            {
                var field = type.GetField(name, bindingAttr);
                if (field != null)
                {
                    return new MemberHandler(field);
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
