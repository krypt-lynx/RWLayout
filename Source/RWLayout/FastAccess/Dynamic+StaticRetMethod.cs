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
        public static Func<TResult> StaticRetMethod<TInstance, TResult>(string methodName, BindingFlags bindingAttr = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            return StaticRetMethod<TResult>(method);
        }

        public static Func<TArg0, TResult> StaticRetMethod<TInstance, TArg0, TResult>(string methodName, BindingFlags bindingAttr = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            return StaticRetMethod<TArg0, TResult>(method);
        }

        public static Func<TArg0, TArg1, TResult> StaticRetMethod<TInstance, TArg0, TArg1, TResult>(string methodName, BindingFlags bindingAttr = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            return StaticRetMethod<TArg0, TArg1, TResult>(method);
        }

        public static Func<TArg0, TArg1, TArg2, TResult> StaticRetMethod<TInstance, TArg0, TArg1, TArg2, TResult>(string methodName, BindingFlags bindingAttr = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            return StaticRetMethod<TArg0, TArg1, TArg2, TResult>(method);
        }

        public static Func<TArg0, TArg1, TArg2, TArg3, TResult> StaticRetMethod<TInstance, TArg0, TArg1, TArg2, TArg3, TResult>(string methodName, BindingFlags bindingAttr = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            return StaticRetMethod<TArg0, TArg1, TArg2, TArg3, TResult>(method);
        }

        public static Func<TArg0, TArg1, TArg2, TArg3, TArg4, TResult> StaticRetMethod<TInstance, TArg0, TArg1, TArg2, TArg3, TArg4, TResult>(string methodName, BindingFlags bindingAttr = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            return StaticRetMethod<TArg0, TArg1, TArg2, TArg3, TArg4, TResult>(method);
        }





        public static Func<TResult> StaticRetMethod<TResult>(MethodInfo method)
        {
            return (Func<TResult>)CreateMethodCaller(typeof(Func<TResult>), method, method.ReturnType, null);
        }

        public static Func<TArg0, TResult> StaticRetMethod<TArg0, TResult>(MethodInfo method)
        {

            return (Func<TArg0, TResult>)CreateMethodCaller(typeof(Func<TArg0, TResult>), method, method.ReturnType, null,
                method.GetParameters().Select(x => x.ParameterType).ToArray());
        }

        public static Func<TArg0, TArg1, TResult> StaticRetMethod<TArg0, TArg1, TResult>(MethodInfo method)
        {
            return (Func<TArg0, TArg1, TResult>)CreateMethodCaller(typeof(Func<TArg0, TArg1, TResult>), method, method.ReturnType, null,
                method.GetParameters().Select(x => x.ParameterType).ToArray());
        }

        public static Func<TArg0, TArg1, TArg2, TResult> StaticRetMethod<TArg0, TArg1, TArg2, TResult>(MethodInfo method)
        {
            return (Func<TArg0, TArg1, TArg2, TResult>)CreateMethodCaller(typeof(Func<TArg0, TArg1, TArg2, TResult>), method, method.ReturnType, null,
                method.GetParameters().Select(x => x.ParameterType).ToArray());
        }

        public static Func<TArg0, TArg1, TArg2, TArg3, TResult> StaticRetMethod<TArg0, TArg1, TArg2, TArg3, TResult>(MethodInfo method)
        {
            return (Func<TArg0, TArg1, TArg2, TArg3, TResult>)CreateMethodCaller(typeof(Func<TArg0, TArg1, TArg2, TArg3, TResult>), method, method.ReturnType, null,
                method.GetParameters().Select(x => x.ParameterType).ToArray());
        }

        public static Func<TArg0, TArg1, TArg2, TArg3, TArg4, TResult> StaticRetMethod<TArg0, TArg1, TArg2, TArg3, TArg4, TResult>(MethodInfo method)
        {
            return (Func<TArg0, TArg1, TArg2, TArg3, TArg4, TResult>)CreateMethodCaller(typeof(Func<TArg0, TArg1, TArg2, TArg3, TArg4, TResult>), method, method.ReturnType, null,
                method.GetParameters().Select(x => x.ParameterType).ToArray());
        }
    }
}
