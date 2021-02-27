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
        public static Func<TInstance, TResult> InstanceRetMethod<TInstance, TResult>(string methodName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TInstance : class
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            return InstanceRetMethod<TInstance, TResult>(method);
        }

        public static Func<TInstance, TArg0, TResult> InstanceRetMethod<TInstance, TArg0, TResult>(string methodName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TInstance : class
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            return InstanceRetMethod<TInstance, TArg0, TResult>(method);
        }

        public static Func<TInstance, TArg0, TArg1, TResult> InstanceRetMethod<TInstance, TArg0, TArg1, TResult>(string methodName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TInstance : class
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            return InstanceRetMethod<TInstance, TArg0, TArg1, TResult>(method);
        }

        public static Func<TInstance, TArg0, TArg1, TArg2, TResult> InstanceRetMethod<TInstance, TArg0, TArg1, TArg2, TResult>(string methodName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TInstance : class
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            return InstanceRetMethod<TInstance, TArg0, TArg1, TArg2, TResult>(method);
        }

        public static Func<TInstance, TArg0, TArg1, TArg2, TArg3, TResult> InstanceRetMethod<TInstance, TArg0, TArg1, TArg2, TArg3, TResult>(string methodName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TInstance : class
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            return InstanceRetMethod<TInstance, TArg0, TArg1, TArg2, TArg3, TResult>(method);
        }

        public static Func<TInstance, TArg0, TArg1, TArg2, TArg3, TArg4, TResult> InstanceRetMethod<TInstance, TArg0, TArg1, TArg2, TArg3, TArg4, TResult>(string methodName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TInstance : class
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            return InstanceRetMethod<TInstance, TArg0, TArg1, TArg2, TArg3, TArg4, TResult>(method);
        }




        public static Func<TInstance, TResult> InstanceRetMethod<TInstance, TResult>(MethodInfo method) where TInstance : class
        {
            return CreateMethodCaller<Func<TInstance, TResult>>(method);
        }

        public static Func<TInstance, TArg0, TResult> InstanceRetMethod<TInstance, TArg0, TResult>(MethodInfo method) where TInstance : class
        {
            return CreateMethodCaller<Func<TInstance, TArg0, TResult>>(method);
        }

        public static Func<TInstance, TArg0, TArg1, TResult> InstanceRetMethod<TInstance, TArg0, TArg1, TResult>(MethodInfo method) where TInstance : class
        {
            return CreateMethodCaller<Func<TInstance, TArg0, TArg1, TResult>>(method);
        }

        public static Func<TInstance, TArg0, TArg1, TArg2, TResult> InstanceRetMethod<TInstance, TArg0, TArg1, TArg2, TResult>(MethodInfo method) where TInstance : class
        {
            return CreateMethodCaller<Func<TInstance, TArg0, TArg1, TArg2, TResult>>(method);
        }

        public static Func<TInstance, TArg0, TArg1, TArg2, TArg3, TResult> InstanceRetMethod<TInstance, TArg0, TArg1, TArg2, TArg3, TResult>(MethodInfo method) where TInstance : class
        {
            return CreateMethodCaller<Func<TInstance, TArg0, TArg1, TArg2, TArg3, TResult>>(method);
        }

        public static Func<TInstance, TArg0, TArg1, TArg2, TArg3, TArg4, TResult> InstanceRetMethod<TInstance, TArg0, TArg1, TArg2, TArg3, TArg4, TResult>(MethodInfo method) where TInstance : class
        {
            return CreateMethodCaller<Func<TInstance, TArg0, TArg1, TArg2, TArg3, TArg4, TResult>>(method);
        }

    }
}
