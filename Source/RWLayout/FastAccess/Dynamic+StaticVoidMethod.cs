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
        public static Action StaticVoidMethod<TInstance>(string methodName, BindingFlags bindingAttr = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            return StaticVoidMethod(method);
        }

        public static Action<TArg0> StaticVoidMethod<TInstance, TArg0>(string methodName, BindingFlags bindingAttr = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            return StaticVoidMethod<TArg0>(method);
        }

        public static Action<TArg0, TArg1> StaticVoidMethod<TInstance, TArg0, TArg1>(string methodName, BindingFlags bindingAttr = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            return StaticVoidMethod<TArg0, TArg1>(method);
        }

        public static Action<TArg0, TArg1, TArg2> StaticVoidMethod<TInstance, TArg0, TArg1, TArg2>(string methodName, BindingFlags bindingAttr = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            return StaticVoidMethod<TArg0, TArg1, TArg2>(method);
        }

        public static Action<TArg0, TArg1, TArg2, TArg3> StaticVoidMethod<TInstance, TArg0, TArg1, TArg2, TArg3>(string methodName, BindingFlags bindingAttr = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            return StaticVoidMethod<TArg0, TArg1, TArg2, TArg3>(method);
        }

        public static Action<TArg0, TArg1, TArg2, TArg3, TArg4> StaticVoidMethod<TInstance, TArg0, TArg1, TArg2, TArg3, TArg4>(string methodName, BindingFlags bindingAttr = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            return StaticVoidMethod< TArg0, TArg1, TArg2, TArg3, TArg4>(method);
        }




        public static Action StaticVoidMethod(MethodInfo method)
        {
            return (Action)CreateMethodCaller(typeof(Action), method, null, null);
        }

        public static Action<TArg0> StaticVoidMethod<TArg0>(MethodInfo method)
        {
            return (Action<TArg0>)CreateMethodCaller(typeof(Action<TArg0>),
                method, null, null, method.GetParameters().Select(x => x.ParameterType).ToArray());
        }

        public static Action<TArg0, TArg1> StaticVoidMethod<TArg0, TArg1>(MethodInfo method)
        {
            return (Action<TArg0, TArg1>)CreateMethodCaller(typeof(Action<TArg0, TArg1>),
                method, null, null, method.GetParameters().Select(x => x.ParameterType).ToArray());
        }

        public static Action<TArg0, TArg1, TArg2> StaticVoidMethod<TArg0, TArg1, TArg2>(MethodInfo method)
        {
            return (Action<TArg0, TArg1, TArg2>)CreateMethodCaller(typeof(Action<TArg0, TArg1, TArg2>),
                method, null, null, method.GetParameters().Select(x => x.ParameterType).ToArray());
        }

        public static Action<TArg0, TArg1, TArg2, TArg3> StaticVoidMethod<TArg0, TArg1, TArg2, TArg3>(MethodInfo method)
        {
            return (Action<TArg0, TArg1, TArg2, TArg3>)CreateMethodCaller(typeof(Action<TArg0, TArg1, TArg2, TArg3>),
                method, null, null, method.GetParameters().Select(x => x.ParameterType).ToArray());
        }

        public static Action<TArg0, TArg1, TArg2, TArg3, TArg4> StaticVoidMethod<TArg0, TArg1, TArg2, TArg3, TArg4>(MethodInfo method)
        {
            return (Action<TArg0, TArg1, TArg2, TArg3, TArg4>)CreateMethodCaller(typeof(Action<TArg0, TArg1, TArg2, TArg3, TArg4>),
                method, null, null, method.GetParameters().Select(x => x.ParameterType).ToArray());
        }
    }
}
