using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RWLayout.alpha2.FastAccess
{
    public partial class Dynamic
    {
        public static Action<TInstance> InstanceVoidMethod<TInstance>(string methodName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TInstance : class
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            if (method == null)
            {
                throw new MemberNotFoundException(methodName, typeof(TInstance));
            }
            return InstanceVoidMethod<TInstance>(method);
        }

        public static Action<TInstance, TArg0> InstanceVoidMethod<TInstance, TArg0>(string methodName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TInstance : class
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            if (method == null)
            {
                throw new MemberNotFoundException(methodName, typeof(TInstance));
            }
            return InstanceVoidMethod<TInstance, TArg0>(method);
        }

        public static Action<TInstance, TArg0, TArg1> InstanceVoidMethod<TInstance, TArg0, TArg1>(string methodName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TInstance : class
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            if (method == null)
            {
                throw new MemberNotFoundException(methodName, typeof(TInstance));
            }
            return InstanceVoidMethod<TInstance, TArg0, TArg1>(method);
        }

        public static Action<TInstance, TArg0, TArg1, TArg2> InstanceVoidMethod<TInstance, TArg0, TArg1, TArg2>(string methodName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TInstance : class
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            if (method == null)
            {
                throw new MemberNotFoundException(methodName, typeof(TInstance));
            }
            return InstanceVoidMethod<TInstance, TArg0, TArg1, TArg2>(method);
        }

        public static Action<TInstance, TArg0, TArg1, TArg2, TArg3> InstanceVoidMethod<TInstance, TArg0, TArg1, TArg2, TArg3>(string methodName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TInstance : class
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            if (method == null)
            {
                throw new MemberNotFoundException(methodName, typeof(TInstance));
            }
            return InstanceVoidMethod<TInstance, TArg0, TArg1, TArg2, TArg3>(method);
        }

        public static Action<TInstance, TArg0, TArg1, TArg2, TArg3, TArg4> InstanceVoidMethod<TInstance, TArg0, TArg1, TArg2, TArg3, TArg4>(string methodName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TInstance : class
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            if (method == null)
            {
                throw new MemberNotFoundException(methodName, typeof(TInstance));
            }
            return InstanceVoidMethod<TInstance, TArg0, TArg1, TArg2, TArg3, TArg4>(method);
        }




        public static Action<TInstance> InstanceVoidMethod<TInstance>(MethodInfo method) where TInstance : class
        {
            return CreateMethodCaller<Action<TInstance>>(method);
        }

        public static Action<TInstance, TArg0> InstanceVoidMethod<TInstance, TArg0>(MethodInfo method) where TInstance : class
        {
            return CreateMethodCaller<Action<TInstance, TArg0>>(method);
        }

        public static Action<TInstance, TArg0, TArg1> InstanceVoidMethod<TInstance, TArg0, TArg1>(MethodInfo method) where TInstance : class
        {
            return CreateMethodCaller<Action<TInstance, TArg0, TArg1>>(method);
        }

        public static Action<TInstance, TArg0, TArg1, TArg2> InstanceVoidMethod<TInstance, TArg0, TArg1, TArg2>(MethodInfo method) where TInstance : class
        {
            return CreateMethodCaller<Action<TInstance, TArg0, TArg1, TArg2>>(method);
        }

        public static Action<TInstance, TArg0, TArg1, TArg2, TArg3> InstanceVoidMethod<TInstance, TArg0, TArg1, TArg2, TArg3>(MethodInfo method) where TInstance : class
        {
            return CreateMethodCaller<Action<TInstance, TArg0, TArg1, TArg2, TArg3>>(method);
        }

        public static Action<TInstance, TArg0, TArg1, TArg2, TArg3, TArg4> InstanceVoidMethod<TInstance, TArg0, TArg1, TArg2, TArg3, TArg4>(MethodInfo method) where TInstance : class
        {
            return CreateMethodCaller<Action<TInstance, TArg0, TArg1, TArg2, TArg3, TArg4>>(method);
        }

    }
}
