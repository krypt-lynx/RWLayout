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
        public static Action StaticVoidMethod(MethodInfo method)
        {
            return (Action)CreateMethodCaller(typeof(Action), method, method.ReturnType);
        }

        public static Action<TArg0> StaticVoidMethod<TArg0>(MethodInfo method)
        {
            return (Action<TArg0>)CreateMethodCaller(typeof(Action<TArg0>), method, null,
                method.GetParameters().Select(x => x.ParameterType).ToArray());
        }

        public static Action<TArg0, TArg1> StaticVoidMethod<TArg0, TArg1>(MethodInfo method)
        {
            return (Action<TArg0, TArg1>)CreateMethodCaller(typeof(Action<TArg0, TArg1>), method, null,
                method.GetParameters().Select(x => x.ParameterType).ToArray());
        }

        public static Action<TArg0, TArg1, TArg2> StaticVoidMethod<TArg0, TArg1, TArg2>(MethodInfo method)
        {
            return (Action<TArg0, TArg1, TArg2>)CreateMethodCaller(typeof(Action<TArg0, TArg1, TArg2>), method, null,
                method.GetParameters().Select(x => x.ParameterType).ToArray());
        }

        public static Action<TArg0, TArg1, TArg2, TArg3> StaticVoidMethod<TArg0, TArg1, TArg2, TArg3>(MethodInfo method)
        {
            return (Action<TArg0, TArg1, TArg2, TArg3>)CreateMethodCaller(typeof(Action<TArg0, TArg1, TArg2, TArg3>), method, null,
                method.GetParameters().Select(x => x.ParameterType).ToArray());
        }

        public static Action<TArg0, TArg1, TArg2, TArg3, TArg4> StaticVoidMethod<TArg0, TArg1, TArg2, TArg3, TArg4>(MethodInfo method)
        {
            return (Action<TArg0, TArg1, TArg2, TArg3, TArg4>)CreateMethodCaller(typeof(Action<TArg0, TArg1, TArg2, TArg3, TArg4>), method, null,
                method.GetParameters().Select(x => x.ParameterType).ToArray());
        }
    }
}
